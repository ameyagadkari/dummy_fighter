using Links;
using UnityEngine;

namespace Dummy
{
    public class DummyController : MonoBehaviour
    {
        public enum DummyStates : byte
        {
            Idle,
            Attack,
            Dodge
        }

        public DummyStates[] DummyStatesArray { get; private set; }
        public int CurrentStateNumber { get; private set; }
        private Animator _animator;
        private static readonly int AttackTriggerId = Animator.StringToHash("Attack");
        private static readonly int DodgeTriggerId = Animator.StringToHash("Dodge");
        private static readonly int ReplayTriggerId = Animator.StringToHash("Replay");
        private static readonly int LoseTriggerId = Animator.StringToHash("Lose");
        private static readonly int WinTriggerId = Animator.StringToHash("Win");

        private void Start()
        {
            _animator = GetComponent<Animator>();
            CurrentStateNumber = 0;
        }

        public void ReplayGame()
        {
            CurrentStateNumber = 0;
            Manager.Instance.ResetManager();
            _animator.ResetTrigger(AttackTriggerId);
            _animator.ResetTrigger(DodgeTriggerId);
            _animator.ResetTrigger(LoseTriggerId);
            _animator.ResetTrigger(WinTriggerId);
            _animator.SetTrigger(ReplayTriggerId);
        }

        private void SetNextState()
        {
            if (CurrentStateNumber >= DummyStatesArray.Length) return;
            Manager.Instance.HasGameStarted = true;
            //print(transform.parent.name + " ... " + DummyStatesArray[_currentStateNumber]);
            switch (DummyStatesArray[CurrentStateNumber])
            {
                case DummyStates.Idle:
                    break;
                case DummyStates.Attack:
                    _animator.SetTrigger(AttackTriggerId);
                    break;
                case DummyStates.Dodge:
                    _animator.SetTrigger(DodgeTriggerId);
                    break;
            }
            CurrentStateNumber++;
        }

        public void SetWinLoseState(bool didWin)
        {
            _animator.SetTrigger(didWin ? WinTriggerId : LoseTriggerId);
        }

        public void ProcessLinks(ref LinkInfo[] player, ref LinkInfo[] enemy)
        {
            DummyStatesArray = new DummyStates[player.Length];
            var length = player.Length;
            for (var i = 0; i < length; i++)
            {
                switch (player[i].LinkTypeName)
                {
                    case LinkInfo.LinkType.Think:
                        DummyStatesArray[i] = DummyStates.Idle;
                        if (i < length - 1)
                        {
                            ProcessInCaseValues(i, length, player[i + 1].LinkTypeName, ref player);
                        }
                        break;
                    case LinkInfo.LinkType.Watch:
                        DummyStatesArray[i] = DummyStates.Idle;
                        if (Random.value > 0.5f && i < length - 1)
                        {
                            ProcessInCaseValues(i, length, enemy[i + 1].LinkTypeName, ref player);
                        }
                        break;
                    case LinkInfo.LinkType.Attack:
                        DummyStatesArray[i] = DummyStates.Attack;
                        break;
                    case LinkInfo.LinkType.Dodge:
                        DummyStatesArray[i] = DummyStates.Dodge;
                        break;
                }
            }
        }

        private static void ProcessInCaseValues(int currentValue, int arrayLength, LinkInfo.LinkType linkTypeName, ref LinkInfo[] dummy)
        {
            switch (linkTypeName)
            {
                case LinkInfo.LinkType.Think:
                case LinkInfo.LinkType.Watch:
                    dummy[currentValue + 1] = dummy[CalculateIndex(currentValue + 1, dummy[currentValue].InCaseValues.Idle, arrayLength)];
                    break;
                case LinkInfo.LinkType.Attack:
                    dummy[currentValue + 1] = dummy[CalculateIndex(currentValue + 1, dummy[currentValue].InCaseValues.Attack, arrayLength)];
                    break;
                case LinkInfo.LinkType.Dodge:
                    dummy[currentValue + 1] = dummy[CalculateIndex(currentValue + 1, dummy[currentValue].InCaseValues.Dodge, arrayLength)];
                    break;
            }
        }

        private static int CalculateIndex(int currentIndex, int inCaseValue, int length)
        {
            if (inCaseValue == 0) return currentIndex;
            var returnValue = inCaseValue < 0
                ? currentIndex - (inCaseValue % length)
                : currentIndex + (inCaseValue % length);
            return returnValue < 0
                ? returnValue + length
                : (returnValue >= length ? returnValue - length : returnValue);
        }
    }
}
