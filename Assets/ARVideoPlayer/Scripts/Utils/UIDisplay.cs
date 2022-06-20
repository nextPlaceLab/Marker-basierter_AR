using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace nextPlace.ARVideoPlayer
{
    internal class UIDisplay : MonoBehaviour
    {
        public static UIDisplay current;

        [SerializeField]
        private TextMeshProUGUI labelText;

        [SerializeField]
        private TextMeshProUGUI valuesText;

        private Image TrackingState;

        private Dictionary<string, string> data = new Dictionary<string, string>();
        private TrackingState _trackingState;
        private TrackingState _currentTrackingState;

        private void Awake()
        {
            current = this;
        }

        public void SetTrackingState(TrackingState state)
        {
            _trackingState = state;
        }

        private void UpdateTrackingState(TrackingState state)
        {
            _currentTrackingState = state;
            if (TrackingState == null) return;

            switch (state)
            {
                case ARVideoPlayer.TrackingState.Nothing:
                    TrackingState.color = Color.red;
                    break;

                case ARVideoPlayer.TrackingState.Update:
                    TrackingState.color = Color.green;
                    break;

                case ARVideoPlayer.TrackingState.Interpolation:
                    TrackingState.color = Color.blue;

                    break;

                default:
                    break;
            }
        }

        public void Set(string label, object value)
        {
            if (data.ContainsKey(label))
                data[label] = value.ToString();
            else
                data.Add(label, value.ToString());
        }

        public bool Remove(string label)
        {
            return data.Remove(label);
        }

        private void Update()
        {
            var labels = "";//"Name:\n";
            var values = "";// Wert:\n";
            foreach (var item in data)
            {
                labels += item.Key + ":\n";
                values += item.Value + "\n";
            }
            labelText.text = labels;
            valuesText.text = values;

            if (_currentTrackingState != _trackingState)
                UpdateTrackingState(_trackingState);
        }
    }
}