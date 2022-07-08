using System.Collections.Generic;
using System.Text;

namespace VCLWebAPI.Models.TransferMatrixMethod.AcousticCalculation
{
    public class FacadeComponent
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public FacadeComponent()
        {
            Rooms = new List<Room>();
            Plates = new List<Plate>();
        }

        /// <summary>
        /// Add a room to the input.
        /// </summary>
        /// <param name="room"></param>
        public void Add(Room room)
        {
            Rooms.Add(room);
        }

        /// <summary>
        /// Add a plate to the input.
        /// </summary>
        /// <param name="plate"></param>
        public void Add(Plate plate)
        {
            Plates.Add(plate);
        }

        /// <summary>
        /// Dump input as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var res = new StringBuilder();
            res.Append("FacadeComponent:\n rooms = \n");
            foreach (var room in Rooms)
            {
                res.Append(room.ToString() + "\n");
            }
            res.Append("\n plates = \n");
            foreach (var plate in Rooms)
            {
                res.Append(plate.ToString() + "\n");
            }
            return res.ToString();
        }

        // DATA

        public double RhoAir { get; set; }
        public List<Room> Rooms { get; set; }
        public List<Plate> Plates { get; set; }
    }
}