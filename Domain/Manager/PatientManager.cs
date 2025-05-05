using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Manager
{
    public class PatientManager : IPatientManager
    {
        private readonly string _filePath;
        private readonly List<Patient> _patients;
        private static readonly string[] _bloodGroups = {
            "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"
        };

        public PatientManager(string filePath)
        {
            _filePath = filePath;
            _patients = File.Exists(_filePath)
                ? File.ReadAllLines(_filePath)
                      .Select(line => line.Split(','))
                      .Select(p => new Patient
                      {
                          Name = p[0],
                          LastName = p[1],
                          CI = p[2],
                          BloodGroup = p[3]
                      })
                      .ToList()
                : new List<Patient>();
        }

        public IEnumerable<Patient> GetAll() => _patients;

        public Patient GetByCi(string ci) =>
            _patients.FirstOrDefault(p => p.CI == ci);

        public void Create(Patient patient)
        {
            // Asignación aleatoria de grupo sangue
            var rnd = new Random();
            patient.BloodGroup = _bloodGroups[rnd.Next(_bloodGroups.Length)];

            _patients.Add(patient);
            Persist();
        }

        public bool Update(string ci, string name, string lastName)
        {
            var existing = GetByCi(ci);
            if (existing == null) return false;
            existing.Name = name;
            existing.LastName = lastName;
            Persist();
            return true;
        }

        public bool Delete(string ci)
        {
            var existing = GetByCi(ci);
            if (existing == null) return false;
            _patients.Remove(existing);
            Persist();
            return true;
        }

        private void Persist() =>
            File.WriteAllLines(_filePath,
                _patients.Select(p =>
                    $"{p.Name},{p.LastName},{p.CI},{p.BloodGroup}"
                )
            );
    }
}
