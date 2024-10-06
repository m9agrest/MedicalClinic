namespace MedicalClinic.Models
{
    public class HtmlHuman
    {
        public Human? Human { get; set; }
        public List<DoctorType> DoctorTypes { get; set; } = new List<DoctorType>();
        public List<int> SelectedDoctorTypeIds { get; set; } = new List<int>();
        public bool isEditor = false;
    }
    public class HtmlService
    {
        public Service? Service { get; set; }
        public List<Human>? Doctors { get; set; }
        public List<DoctorType> DoctorTypes { get; set; } = new List<DoctorType>();
        public List<int> SelectedDoctorTypeIds { get; set; } = new List<int>();
        public bool isEditor = false;
    }
    public class ServiceRecordViewModel
    {
        public Human? Doctor { get; set; }
        public Service? Service { get; set; }
        public List<string> AvailableTimeSlots { get; set; } = new List<string>(); // Список доступных временных слотов
    }


}
