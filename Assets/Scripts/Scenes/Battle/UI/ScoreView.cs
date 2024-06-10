using Core.Shared.Foundation;
using Core.Shared.ReactiveExtensions;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Battle.UI
{
    public class ScoreView : BaseMonoBehaviour
    {
        public struct Ctx
		{
            public CompositeDisposable viewDisposable;
            public IReadOnlyReactiveTrigger increaseScore;
        }

        [SerializeField] private Text _score_text = null;

        private Ctx _ctx;
        private int _score;

        public void SetCtx(Ctx ctx)
		{
            _ctx = ctx;
            _score = 0;
            _score_text.text = _score.ToString();

            _ctx.viewDisposable.Add(_ctx.increaseScore.Subscribe(() =>
            {
                _score++;
                _score_text.text = _score.ToString();
            }));
		}
    }
}
