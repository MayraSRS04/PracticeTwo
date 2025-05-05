using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Manager
{
    public interface IPatientManager
    {
        IEnumerable<Patient> GetAll();
        Patient GetByCi(string ci);
        void Create(Patient patient);
        bool Update(string ci, string name, string lastName);
        bool Delete(string ci);
    }
}
