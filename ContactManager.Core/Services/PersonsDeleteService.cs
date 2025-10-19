﻿using ContactManager.Core.Domain.RepositoryContracts;
using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.Extensions.Logging;
using Serilog;
using ServiceContracts;


namespace Services
{
    public class PersonsDeleteService : IPersonsDeleteService
    {
        //private field
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        //constructor
        public PersonsDeleteService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }


        public async Task<bool> DeletePerson(Guid? personID)
        {
            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }

            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);
            if (person == null)
                return false;

            await _personsRepository.DeletePersonByPersonID(personID.Value);

            return true;
        }


    }
}
