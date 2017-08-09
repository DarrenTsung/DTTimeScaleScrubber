using System.Collections;
using System.Linq;
using UnityEngine;

using DTTimeScaleScrubber.Internal;

#if IN_CONTROL
using InControl;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DTTimeScaleScrubber {
	public class TimeScaleScrubber : MonoBehaviour {
		// PRAGMA MARK - Public Interface
		public bool IsScrubbing {
			get; private set;
		}


		// PRAGMA MARK - Internal
		// in viewport distance (1.0f == whole screen)
		private const float kTouchScrubMaxDistance = 0.45f;
		private const float kTouchScrubMinDistance = 0.15f;

		[Header("Outlets")]
		[SerializeField]
		private GameObject viewPrefab_;

		[Header("Properties")]
		[SerializeField]
		private float timeScaleMax_ = 20.0f;
		[SerializeField]
		private float timeScaleMin_ = 0.1f;

		[Space]
		[SerializeField]
		private KeyCode slowTimeKey_ = KeyCode.Q;
		[SerializeField]
		private KeyCode speedTimeKey_ = KeyCode.E;

		#if IN_CONTROL
		[Space]
		[SerializeField]
		private InputControlType slowTimeControl_ = InputControlType.LeftBumper;
		[SerializeField]
		private InputControlType speedTimeControl_ = InputControlType.RightBumper;
		#endif

		private GameObject view_;

		private void Awake() {
			bool debug = Debug.isDebugBuild;
			#if DEBUG
			debug = true;
			#endif

			if (!debug) {
				this.enabled = false;
				return;
			}

			GameObject.DontDestroyOnLoad(this.gameObject);

			view_ = GameObject.Instantiate(viewPrefab_);
			view_.transform.SetParent(this.transform);
			view_.GetComponent<TimeScaleScrubberView>().Init(this);

			StartCoroutine(UpdateTimeScaleScrubbing());
		}

		#if UNITY_EDITOR
		private void Reset() {
			viewPrefab_ = AssetDatabaseUtil.LoadSpecificAssetNamed<GameObject>("TimeScaleScrubberView");
		}
		#endif

		private void OnDestroy() {
			if (view_ != null) {
				GameObject.Destroy(view_);
				view_ = null;
			}
		}

		private IEnumerator UpdateTimeScaleScrubbing() {
			bool shouldResetTimeScale = false;
			int previousNumberOfTouches = 0;
			Vector2 scrubTouchStartingCenter = Vector2.zero;

			while (true) {
				float timeScaleScrub = 0.0f;

				if (Input.GetKey(speedTimeKey_)) {
					timeScaleScrub = 1.0f;
				} else if (Input.GetKey(slowTimeKey_)) {
					timeScaleScrub = -1.0f;
				}

				#if IN_CONTROL
				foreach (InputDevice device in InputManager.Devices) {
					if (device.GetControl(speedTimeControl_).IsPressed) {
						timeScaleScrub = 1.0f;
					} else if (device.GetControl(slowTimeControl_).IsPressed) {
						timeScaleScrub = -1.0f;
					}
				}
				#endif

				int numberOfTouches = Input.touches.Length;
				if (numberOfTouches == 2) {
					Vector2 touchPixelCenter = Input.touches.Select(t => t.position).Average();
					Vector2 touchCenter = ScreenUtil.ScreenToViewportPoint(touchPixelCenter);
					if (numberOfTouches != previousNumberOfTouches) {
						// start scrubbing
						scrubTouchStartingCenter = touchCenter;
					}

					float yOffset = touchCenter.y - scrubTouchStartingCenter.y;
					if (yOffset > 0.0f) {
						timeScaleScrub = Mathf.Min(yOffset, kTouchScrubMaxDistance) / kTouchScrubMaxDistance;
					} else if (yOffset < 0.0f) {
						timeScaleScrub = Mathf.Max(yOffset, -kTouchScrubMinDistance) / kTouchScrubMinDistance;
					}
				}


				if (timeScaleScrub != 0.0f) {
					shouldResetTimeScale = true;

					if (timeScaleScrub > 0) {
						Time.timeScale = Mathf.Lerp(1.0f, timeScaleMax_, timeScaleScrub);
					} else if (timeScaleScrub < 0) {
						Time.timeScale = Mathf.Lerp(1.0f, timeScaleMin_, -timeScaleScrub);
					}
				} else if (shouldResetTimeScale) {
					shouldResetTimeScale = false;
					Time.timeScale = 1.0f;
				}

				IsScrubbing = timeScaleScrub != 0.0f;
				previousNumberOfTouches = numberOfTouches;
				yield return null;
			}
		}
	}
}