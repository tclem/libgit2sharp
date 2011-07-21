using System;

namespace LibGit2Sharp
{
    [Flags]
    public enum GitStatus
    {
        GIT_STATUS_NOTFOUND =       -1,
        GIT_STATUS_CURRENT =        0,
        GIT_STATUS_INDEX_NEW =      (1 << 0),
        GIT_STATUS_INDEX_MODIFIED = (1 << 1),
        GIT_STATUS_INDEX_DELETED =  (1 << 2),
        GIT_STATUS_WT_NEW =         (1 << 3),
        GIT_STATUS_WT_MODIFIED =    (1 << 4),
        GIT_STATUS_WT_DELETED =     (1 << 5),

        //TODO: Ignored files not handled yet
        GIT_STATUS_IGNORED =        (1 << 6),
    }
}