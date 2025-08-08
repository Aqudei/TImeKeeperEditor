using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKeeperEditor.Models
{
    public class Log : BindableBase
    {
        private string _logRefNo;
        public string LogRefNo
        {
            get => _logRefNo;
            set => SetProperty(ref _logRefNo, value);
        }

        private string _logId;
        public string LogId
        {
            get => _logId;
            set => SetProperty(ref _logId, value);
        }

        private string _logMethod;
        public string LogMethod
        {
            get => _logMethod;
            set => SetProperty(ref _logMethod, value);
        }

        private string _logDate;
        public string LogDate
        {
            get => _logDate;
            set => SetProperty(ref _logDate, value);
        }

        private string _logInAM;
        public string LogInAM
        {
            get => _logInAM;
            set => SetProperty(ref _logInAM, value);
        }

        private string _logOutAM;
        public string LogOutAM
        {
            get => _logOutAM;
            set => SetProperty(ref _logOutAM, value);
        }

        private string _logInPM;
        public string LogInPM
        {
            get => _logInPM;
            set => SetProperty(ref _logInPM, value);
        }

        private string _logOutPM;
        public string LogOutPM
        {
            get => _logOutPM;
            set => SetProperty(ref _logOutPM, value);
        }

        private string _logDevice;
        public string LogDevice
        {
            get => _logDevice;
            set => SetProperty(ref _logDevice, value);
        }

        private string _schedIn;
        public string SchedIn
        {
            get => _schedIn;
            set => SetProperty(ref _schedIn, value);
        }

        private string _schedOut;
        public string SchedOut
        {
            get => _schedOut;
            set => SetProperty(ref _schedOut, value);
        }

        private int _tardyMin;
        public int TardyMin
        {
            get => _tardyMin;
            set => SetProperty(ref _tardyMin, value);
        }

        private int _tardyHour;
        public int TardyHour
        {
            get => _tardyHour;
            set => SetProperty(ref _tardyHour, value);
        }

        private string _leaveReference;
        public string LeaveReference
        {
            get => _leaveReference;
            set => SetProperty(ref _leaveReference, value);
        }

        private int _adminId;
        public int AdminId
        {
            get => _adminId;
            set => SetProperty(ref _adminId, value);
        }

        private string _actualIN;
        public string ActualIN
        {
            get => _actualIN;
            set => SetProperty(ref _actualIN, value);
        }

        private string _actualOUT;
        public string ActualOUT
        {
            get => _actualOUT;
            set => SetProperty(ref _actualOUT, value);
        }

        private string _remarks;
        public string Remarks
        {
            get => _remarks;
            set => SetProperty(ref _remarks, value);
        }

        private DateTime? _datePosted;
        public DateTime? DatePosted
        {
            get => _datePosted;
            set => SetProperty(ref _datePosted, value);
        }

        private string _pid;
        public string PID
        {
            get => _pid;
            set => SetProperty(ref _pid, value);
        }
    }
}
