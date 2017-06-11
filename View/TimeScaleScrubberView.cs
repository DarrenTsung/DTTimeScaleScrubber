using UnityEngine;

namespace DTTimeScaleScrubber.Internal {
	public class TimeScaleScrubberView : MonoBehaviour {
		// PRAGMA MARK - Public Interface
		public void Init(TimeScaleScrubber timeScaleScrubber) {
			timeScaleScrubber_ = timeScaleScrubber;
		}


		// PRAGMA MARK - Internal
		[Header("Outlets")]
		[SerializeField]
		private TextOutlet timeScaleText_;
		[SerializeField]
		private CanvasGroup canvasGroup_;

		private TimeScaleScrubber timeScaleScrubber_;

		private void Update() {
			if (!timeScaleScrubber_.IsScrubbing) {
				canvasGroup_.alpha = 0.0f;
				return;
			}

			float timeScale = Time.timeScale;
			if (!Mathf.Approximately(timeScale, 1.0f)) {
				canvasGroup_.alpha = Mathf.Clamp(Mathf.Abs(timeScale - 1.0f), 0.0f, 1.0f);
				timeScaleText_.Text = string.Format("x{0:0.#}", timeScale);
			} else {
				canvasGroup_.alpha = 0.0f;
			}
		}
	}
}