using Links;
using UnityEngine;

namespace Dummy
{
    public class DummyController : MonoBehaviour
    {
        public enum DummyStates : byte { Idle, Attack, Dodge }

        private DummyStates[] _actorStates;
        public void ProcessLinks(ref LinkInfo[] player, ref LinkInfo[] enemy)
        {
            _actorStates = new DummyStates[player.Length];
            var length = player.Length;
            for (var i = 0; i < length; i++)
            {
                switch (player[i].LinkTypeName)
                {
                    case LinkInfo.LinkType.Think:
                        _actorStates[i] = DummyStates.Idle;
                        if (i < length - 1)
                        {
                            ProcessInCaseValues(i, length, player[i + 1].LinkTypeName, ref player);
                        }
                        break;
                    case LinkInfo.LinkType.Watch:
                        _actorStates[i] = DummyStates.Idle;
                        if (Random.value > 0.5f && i < length - 1)
                        {
                            ProcessInCaseValues(i, length, enemy[i + 1].LinkTypeName, ref player);
                        }
                        break;
                    case LinkInfo.LinkType.Attack:
                        _actorStates[i] = DummyStates.Attack;
                        break;
                    case LinkInfo.LinkType.Dodge:
                        _actorStates[i] = DummyStates.Dodge;
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
