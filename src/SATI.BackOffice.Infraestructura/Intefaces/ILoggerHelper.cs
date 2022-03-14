using System;
using System.Diagnostics;

namespace SATI.BackOffice.Infraestructura.Intefaces
{
    public interface ILoggerHelper
    {
        Exception Log(Exception ex);
        void Log(TraceEventType tipo, string mensaje);
    }
}
