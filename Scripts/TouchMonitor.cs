using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void EventDelegate( TouchEvent ev );

public enum TouchType 
{
	Start,
	Hold,
	Stop,
	Tap,
	Swipe,
	LongPress, //not implemented
	Focus,
	Blur
}

public enum SwipeType 
{
	Up,
	Down,
	Left,
	Right
}

public enum HandlerType 
{
	FullScreen,
	RendererBased,
	RectBased,
	Dynamic
}

public class TouchEvent 
{
	public int fingerId;
	public TouchType type;
	public SwipeType swipeType;
	public Vector3 position;
	public Vector3 deltaPosition;
	public bool handled;
}

public class EventHandler 
{
	public EventDelegate eventDelegate;
	public HandlerType handlerType;
	public Renderer renderer;
	public Camera camera;
	public int fingerTouching;
	public Rect touchArea;
	public GameState gameState;
	public Transform followTransform;
	public GameObject followTransformGO;
	public Vector3 extents;
	public float bufferMod;
}

public class TouchTrack 
{
	public float touchTime;
	public Vector3 touchPosition;
	//public bool longPressed;
}

public class TouchMonitor : MonoBehaviour
{
	static TouchMonitor _this;
	
	public float tapTime = 0.35f;
	public float swipeTime = 0.5f;
	public float swipePower = 60000;
	public float tapDistance = 5000;
	//public float longPressTime = 1.0f;
	
	List<EventHandler> eventHandlers = new List<EventHandler>();
	List<EventHandler> activeHandlers;
	EventHandler fallBackHandler = null;
	Dictionary<int, TouchTrack> touchTracker = new Dictionary<int, TouchTrack>();
	List<TouchEvent> events = new List<TouchEvent>();
#if UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_DASHBOARD_WIDGET
	Vector3 lastMousePosition;
#endif
	
	void Awake() {	
		if (_this) { Debug.Log("Only one TouchMonitor can exist in the scene"); return; }
		_this = this;
	}
	
	#region Register Overloads
	public static void Register( EventDelegate evDlg ) {
		_this.registerHandler(evDlg, HandlerType.FullScreen, null, null, Vector2.zero, Vector2.zero, GameState.None);	
	}
	
	public static void RegisterFallback( EventDelegate evDlg ) {
		_this.fallBackHandler = new EventHandler() { eventDelegate = evDlg, handlerType = HandlerType.FullScreen, fingerTouching = -1 };
	}
	
	public static void Register( EventDelegate evDlg, Renderer renderer) {
		_this.registerHandler(evDlg, HandlerType.RendererBased, renderer, Camera.main, Vector2.zero, Vector2.zero, GameState.None);	
	}

	public static void Register( EventDelegate evDlg, Renderer renderer, Camera camera ) {
		_this.registerHandler(evDlg, HandlerType.RendererBased, renderer, camera, Vector2.zero, Vector2.zero, GameState.None);	
	}
	
	public static void Register( EventDelegate evDlg, Vector2 topLeft, Vector2 widthHeight ) {
		_this.registerHandler(evDlg, HandlerType.RectBased, null, null, topLeft, widthHeight, GameState.None);	
	}

	public static void Register( EventDelegate evDlg, GameState gameState ) {
		_this.registerHandler(evDlg, HandlerType.FullScreen, null, null, Vector2.zero, Vector2.zero, gameState);	
	}
	
	public static void RegisterFallback( EventDelegate evDlg, GameState gs ) {
		_this.fallBackHandler = new EventHandler() { eventDelegate = evDlg, handlerType = HandlerType.FullScreen, fingerTouching = -1 };
	}
	
	public static void Register( EventDelegate evDlg, Renderer renderer, GameState gameState ) {
		_this.registerHandler(evDlg, HandlerType.RendererBased, renderer, Camera.main, Vector2.zero, Vector2.zero, gameState);	
	}

	public static void Register( EventDelegate evDlg, Renderer renderer, Camera camera, GameState gameState ) {
		_this.registerHandler(evDlg, HandlerType.RendererBased, renderer, camera, Vector2.zero, Vector2.zero, gameState);	
	}
	
	public static void Register( EventDelegate evDlg, Vector2 topLeft, Vector2 widthHeight, GameState gameState ) {
		_this.registerHandler(evDlg, HandlerType.RectBased, null, null, topLeft, widthHeight, gameState);	
	}
	
	public static void Register( EventDelegate evDlg, Transform centerTransform, Vector3 ext, Camera cam, GameState gameState, float bufferFactor = 1 ) {
//		Debug.Log (Time.realtimeSinceStartup + " im here");
		_this.eventHandlers.Add (new EventHandler() { eventDelegate = evDlg, handlerType = HandlerType.Dynamic, fingerTouching = -1, gameState = gameState, followTransform = centerTransform, followTransformGO = centerTransform.gameObject,extents = ext, camera = cam, bufferMod = bufferFactor } );
		SetActiveHandlers();
	}
	#endregion

	void registerHandler( EventDelegate startDlg, HandlerType hType, Renderer r, Camera c, Vector2 topLeft, Vector2 widthHeight, GameState gs ) {
		if (hType == HandlerType.FullScreen) 
			eventHandlers.Add(new EventHandler() { eventDelegate = startDlg, handlerType = hType, fingerTouching = -1, gameState = gs });
		else if (hType == HandlerType.RendererBased) 
			eventHandlers.Add(new EventHandler() { eventDelegate = startDlg, handlerType = hType, renderer = r, camera = c, fingerTouching = -1, gameState = gs });
		else if (hType == HandlerType.RectBased) {
			eventHandlers.Add(new EventHandler() { eventDelegate = startDlg, handlerType = hType, touchArea = new Rect(topLeft.x, topLeft.y, widthHeight.x, widthHeight.y), fingerTouching = -1, gameState = gs});		
		}
		setActiveHandlers();
	}
	
	void setActiveHandlers( GameState state ) {
		activeHandlers = eventHandlers.FindAll( item => item.gameState == state );
	}
	
	void setActiveHandlers() {
		setActiveHandlers( GameEngine.State );
	}
	
	public static void SetActiveHandlers() {
		_this.setActiveHandlers();
	}
	
	public static void Unregister( EventDelegate evDlg ) {
		_this.unregisterHandler(evDlg);
	}
	
	void unregisterHandler( EventDelegate evDlg ) {
		int i = 0;
		int removeIdx = -1;
		for (i = 0; i < eventHandlers.Count; i++) {
			if (eventHandlers[i].eventDelegate == evDlg) {
				removeIdx = i;	
			}
		}
		
		if (removeIdx >= 0) {
			eventHandlers.RemoveAt(removeIdx);
			setActiveHandlers();
		} 
	}
		
	void Update() {
		events.Clear();
		if (Input.touchCount > 0) {
			Touch[] touches = Input.touches;
			
			foreach( Touch touch in touches ) {
				TouchType typ = TouchType.Start;
				
				switch(touch.phase) {
					case TouchPhase.Began: typ = TouchType.Start; break;
					case TouchPhase.Moved: case TouchPhase.Stationary: typ = TouchType.Hold; break;
					case TouchPhase.Canceled: case TouchPhase.Ended:  typ = TouchType.Stop; break;
				}
				
				events.Add(new TouchEvent { fingerId = touch.fingerId, type = typ, position = touch.position, deltaPosition = touch.deltaPosition  });
			}
		}
		
#if UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_DASHBOARD_WIDGET
		if (Input.GetMouseButtonDown(0)) 
			events.Add(new TouchEvent { fingerId = 999, type = TouchType.Start, position = Input.mousePosition, deltaPosition = Vector3.zero });
		else if (Input.GetMouseButtonUp(0)) 
			events.Add(new TouchEvent { fingerId = 999, type = TouchType.Stop, position = Input.mousePosition, deltaPosition = lastMousePosition - Input.mousePosition });
		else if (Input.GetMouseButton(0)) 
			events.Add(new TouchEvent { fingerId = 999, type = TouchType.Hold, position = Input.mousePosition, deltaPosition = lastMousePosition - Input.mousePosition });
		
		lastMousePosition = Input.mousePosition;
#endif
		int eventsCount = events.Count;
		if (eventsCount > 0) {
			TouchEvent lastEvent = events[eventsCount-1];
			
			if (lastEvent.type == TouchType.Start) {
				touchTracker[lastEvent.fingerId] = new TouchTrack() { touchPosition = lastEvent.position, touchTime = Time.time };
			} else if (lastEvent.type == TouchType.Stop) {
				TouchTrack touchVal = null;
				if (touchTracker.TryGetValue(lastEvent.fingerId, out touchVal)) {
					Vector3 delta = touchVal.touchPosition - lastEvent.position;
					if (Time.time - touchVal.touchTime < swipeTime && delta.sqrMagnitude > swipePower) {
						SwipeType swTyp;
						if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) {
							if (delta.x < 0) {
								swTyp = SwipeType.Right;
							} else {
								swTyp = SwipeType.Left;
							}
						} else {
							if (delta.y < 0) {
								swTyp = SwipeType.Up;
							} else {
								swTyp = SwipeType.Down;
							}
						}
						events.Insert(eventsCount - 1, new TouchEvent { fingerId = lastEvent.fingerId, type = TouchType.Swipe, position = touchVal.touchPosition, deltaPosition = lastEvent.deltaPosition, swipeType = swTyp });
					} else if (Time.time - touchVal.touchTime < tapTime && delta.sqrMagnitude < tapDistance) {
						events.Insert(eventsCount - 1, new TouchEvent { fingerId = lastEvent.fingerId, type = TouchType.Tap, position = lastEvent.position, deltaPosition = lastEvent.deltaPosition });
					}
					touchTracker.Remove(lastEvent.fingerId);
				}
			} 
//			else if (lastEvent.type == TouchType.Hold) {
//				TouchTrack touchVal = null;
//				if (touchTracker.TryGetValue(lastEvent.fingerId, out touchVal)) {
//					if (!touchVal.longPressed && Time.time - touchVal.touchTime > longPressTime) {
//						touchVal.longPressed = true;
//						events.Insert(events.Count - 1, new TouchEvent { fingerId = lastEvent.fingerId, type = TouchType.LongPress, position = lastEvent.position, deltaPosition = lastEvent.deltaPosition });
//					}
//				}
//			}
		
			foreach( TouchEvent ev in events ) {
				ev.handled = false;
				foreach( EventHandler handler in activeHandlers ) {
//					if (ev.handled)
//						break; 
				
					if (handler.handlerType == HandlerType.FullScreen) {
						handler.eventDelegate(ev);
						ev.handled = true;
					} else if (handler.handlerType == HandlerType.RendererBased) {
						if (handler.renderer.isVisible) {
							Vector3 max = handler.camera.WorldToScreenPoint(handler.renderer.bounds.center + (handler.renderer.bounds.extents * 1.2f));
							Vector3 min = handler.camera.WorldToScreenPoint(handler.renderer.bounds.center - (handler.renderer.bounds.extents * 1.2f));
							Vector3 diff = max - min;

							if (checkTouch(new Rect(min.x, min.y, diff.x, diff.y), handler, ev)) {
								ev.handled = true;
							}
						}
					} else if (handler.handlerType == HandlerType.RectBased) {
						if (checkTouch(handler.touchArea, handler, ev)) {
							ev.handled = true;
						}
					} else if (handler.handlerType == HandlerType.Dynamic) {
						if (!handler.followTransformGO.active) 
							continue;
						Vector2 max = handler.camera.WorldToScreenPoint(handler.followTransform.position + (handler.extents * handler.bufferMod));
						Vector2 min = handler.camera.WorldToScreenPoint(handler.followTransform.position - (handler.extents * handler.bufferMod));
						Vector2 diff = max - min;

						if (checkTouch(new Rect(min.x, min.y, diff.x, diff.y), handler, ev)) {
							ev.handled = true;
						}
					}
				}
				
				if (!ev.handled) {
//					fallBackHandler.eventDelegate(ev);
					ev.handled = true;	
				}
			}
		}
	}
	
	bool checkTouch(Rect r, EventHandler handler, TouchEvent ev) {
		if (ev.type == TouchType.Stop) {
			if (handler.fingerTouching == ev.fingerId) {
				handler.fingerTouching = -1;
				handler.eventDelegate(ev);
				return true;
			}
		} else {
			if (r.Contains(ev.position)) {
				if (handler.fingerTouching < 0) {
					handler.fingerTouching = ev.fingerId;
					if (ev.type == TouchType.Start) {
						handler.eventDelegate(ev);
					} else {
						handler.eventDelegate( new TouchEvent { fingerId = ev.fingerId, type = TouchType.Focus, position = ev.position, deltaPosition = ev.deltaPosition });
					}
					return true;
				} else if (handler.fingerTouching == ev.fingerId) {
					handler.eventDelegate(ev);
					return true;
				}
			} else if (handler.fingerTouching == ev.fingerId) {
				handler.fingerTouching = -1;
				handler.eventDelegate( new TouchEvent { fingerId = ev.fingerId, type = TouchType.Blur, position = ev.position, deltaPosition = ev.deltaPosition });
				return true;
			}
		}
		
		return false;
	}
}
