﻿using System;

namespace PointToPoint.Messenger.ErrorHandler
{
    /// <summary>
    /// Notifications to the applications about communication problems
    /// </summary>
    public interface IMessengerErrorReporter
    {
        void Disconnected(Guid messengerId, Exception e);
    }
}