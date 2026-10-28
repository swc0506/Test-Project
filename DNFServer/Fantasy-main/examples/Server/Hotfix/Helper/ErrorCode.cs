using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    public class ErrorCode
    {
        public const uint Success = 0;
        public const uint EnterMapFailed_NotFindRole = 1500;

        public const uint EnterMapFailed_NotFindMap = 1501;
        public const uint EnterMapFailed_NotFindDoor = 1502;

        public const uint PlayerMoveFailed_NotFindPlayerInMap = 1502;

        public const uint CreateTeamFailed_AlreadyExitsTeaming = 1510;
        public const uint CreateTeamFailed_NotFindPlayer = 1511;

        public const uint JoinTeamFailed_NotFindTeam = 1515;
        public const uint JoinTeamFailed_OutTeamPlayerCount = 1516;
        public const uint JoinTeamFailed_NotFindPlayer = 1517;
    }
}
