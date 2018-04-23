using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXGrid.CustomFilter {
    class DataSample {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public bool IsCompleted { get; set; }

        public static List<DataSample> GetTasks(int count) {
            Random rnd = new Random();
            List<DataSample> data = new List<DataSample>();
            for(int i = 0; i < count; i++) {
                DataSample ts = new DataSample();
                ts.Id = i;
                ts.Name = "Name-" + i;
                ts.StartDate = DateTime.Now;
                ts.FinishDate = DateTime.Now.AddDays((double)i);
                ts.IsCompleted = rnd.Next(1, 3) == 1 ? true : false;
                data.Add(ts);
            }
            return data;
        }
    }
}
