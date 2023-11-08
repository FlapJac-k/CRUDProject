using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        private readonly ICountryService _countryService;
        private readonly ITestOutputHelper _testOutputHelper;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _countryService = new CountryService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
            _personsService = new PersonsService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options), _countryService);
            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson
        [Fact]
        public void AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            Assert.Throws<ArgumentNullException>(() =>
            {
                _personsService.AddPerson(personAddRequest);
            });
        }

        [Fact]
        public void AddPerson_PersonNameNull()
        {
            //Arrange
            PersonAddRequest personAddRequest = new PersonAddRequest() { PersonName = null };

            //Act
            Assert.Throws<ArgumentException>(() =>
            {
                _personsService.AddPerson(personAddRequest);
            });
        }

        [Fact]
        public void AddPerson_ProperPersonDetails()
        {
            //Arrange
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Person Name...",
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewLetters = true,
                Email = "test@test.com"
            };

            //Act
            PersonResponse person_response_from_add = _personsService.AddPerson(personAddRequest);
            List<PersonResponse> persons_list = _personsService.GetAllPersons();

            //Assert
            Assert.True(person_response_from_add.PersonID != Guid.Empty);
            Assert.Contains(person_response_from_add, persons_list);
        }
        #endregion

        #region GetPersonByPersonID
        [Fact]
        public void GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? PersonID = null;

            //Act
            PersonResponse? person_response_from_get = _personsService.GetPersonByPersonID(PersonID);

            //Assert
            Assert.Null(person_response_from_get);
        }

        [Fact]
        public void GetPersonByPersonID_WithPersonID()
        {
            //Arrange
            CountryAddRequest country_request = new CountryAddRequest() { CountryName = "Egypt" };
            CountryResponse country_response = _countryService.AddCountry(country_request);

            //Act
            PersonAddRequest person_request = new PersonAddRequest()
            {
                PersonName = "test",
                Address = "test",
                Email = "test@test.com",
                CountryID = country_response.CountryId,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewLetters = false,
            };
            PersonResponse person_response_from_add = _personsService.AddPerson(person_request);
            PersonResponse? person_response_from_get = _personsService.GetPersonByPersonID(person_response_from_add.PersonID);

            //Assert
            Assert.Equal(person_response_from_add, person_response_from_get);
        }
        #endregion

        #region GetAllPersons
        [Fact]
        public void FetAllPersons_EmptyList()
        {
            //Act
            List<PersonResponse> persons_from_get = _personsService.GetAllPersons();

            //Assert
            Assert.Empty(persons_from_get);
        }

        [Fact]
        public void GetAllPersons_AddFewPersons()
        {
            //Arrange
            CountryAddRequest country_add_request_1 = new CountryAddRequest() { CountryName = "Egypt" };
            CountryAddRequest country_add_request_2 = new CountryAddRequest() { CountryName = "Cairo" };

            CountryResponse country_response_1 = _countryService.AddCountry(country_add_request_1);
            CountryResponse country_response_2 = _countryService.AddCountry(country_add_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "test",
                Address = "test",
                Email = "test@test.com",
                CountryID = country_response_1.CountryId,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewLetters = false,
            };

            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "test2",
                Address = "test2",
                Email = "test2@test.com",
                CountryID = country_response_2.CountryId,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewLetters = false,
            };


            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            //print
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            List<PersonResponse> persons_from_get = _personsService.GetAllPersons();

            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_from_get)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            foreach (PersonResponse person_response in person_response_list_from_add)
            {
                Assert.Contains(person_response, persons_from_get);
            }


        }
        #endregion

        #region GetFilteredPersons
        [Fact]
        public void GetFilteredPersons_EmptySearchText()
        {
            //Arrange
            CountryAddRequest country_add_request_1 = new CountryAddRequest() { CountryName = "Egypt" };
            CountryAddRequest country_add_request_2 = new CountryAddRequest() { CountryName = "Cairo" };

            CountryResponse country_response_1 = _countryService.AddCountry(country_add_request_1);
            CountryResponse country_response_2 = _countryService.AddCountry(country_add_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "test",
                Address = "test",
                Email = "test@test.com",
                CountryID = country_response_1.CountryId,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewLetters = false,
            };

            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "test2",
                Address = "test2",
                Email = "test2@test.com",
                CountryID = country_response_2.CountryId,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewLetters = false,
            };


            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            //print
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            List<PersonResponse> persons_from_search = _personsService.GetFilteredPersons(nameof(Person.PersonName), "");

            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            foreach (PersonResponse person_response in person_response_list_from_add)
            {
                Assert.Contains(person_response, persons_from_search);
            }


        }

        [Fact]
        public void GetFilteredPersons_EmptySearchByPersonName()
        {
            //Arrange
            CountryAddRequest country_add_request_1 = new CountryAddRequest() { CountryName = "Egypt" };
            CountryAddRequest country_add_request_2 = new CountryAddRequest() { CountryName = "Cairo" };

            CountryResponse country_response_1 = _countryService.AddCountry(country_add_request_1);
            CountryResponse country_response_2 = _countryService.AddCountry(country_add_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "eslam",
                Address = "test",
                Email = "test@test.com",
                CountryID = country_response_1.CountryId,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewLetters = false,
            };

            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "solom",
                Address = "test2",
                Email = "test2@test.com",
                CountryID = country_response_2.CountryId,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewLetters = false,
            };


            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            //print
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            List<PersonResponse> persons_from_search = _personsService.GetFilteredPersons(nameof(Person.PersonName), "es");

            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                if (person_response_from_add.PersonName != null)
                {

                    if (person_response_from_add.PersonName.Contains("es", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(person_response_from_add, persons_from_search);

                    }
                }
            }
        }
        #endregion

        #region GetSortedPersons
        [Fact]
        public void GetSortedPersons()
        {
            //Arrange
            CountryAddRequest country_add_request_1 = new CountryAddRequest() { CountryName = "Egypt" };
            CountryAddRequest country_add_request_2 = new CountryAddRequest() { CountryName = "Cairo" };

            CountryResponse country_response_1 = _countryService.AddCountry(country_add_request_1);
            CountryResponse country_response_2 = _countryService.AddCountry(country_add_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "eslam",
                Address = "test",
                Email = "test@test.com",
                CountryID = country_response_1.CountryId,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewLetters = false,
            };

            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "solom",
                Address = "test2",
                Email = "test2@test.com",
                CountryID = country_response_2.CountryId,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewLetters = false,
            };


            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            //print
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            List<PersonResponse> allPersons = _personsService.GetAllPersons();
            //Act
            List<PersonResponse> persons_from_sort = _personsService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_from_sort)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();

            //Assert
            for (int i = 0; i < person_response_list_from_add.Count; i++)
            {
                Assert.Equal(person_response_list_from_add[i], persons_from_sort[i]);
            }
        }
        #endregion

        #region UpdatePerson
        [Fact]
        public void UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _personsService.UpdatePerson(person_update_request);
            });
        }

        [Fact]
        public void UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = new PersonUpdateRequest()
            {
                PersonID = Guid.NewGuid(),

            };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _personsService.UpdatePerson(person_update_request);
            });
        }

        [Fact]
        public void UpdatePerson_PersonNameIsNull()
        {
            //Arrange
            CountryAddRequest country_request = new CountryAddRequest() { CountryName = "Egypt" };
            CountryResponse country_response = _countryService.AddCountry(country_request);

            PersonAddRequest person_request = new PersonAddRequest()
            {
                PersonName = "test",
                Address = "test",
                Email = "test@test.com",
                CountryID = country_response.CountryId,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewLetters = false,
            };

            PersonResponse person_response_from_add = _personsService.AddPerson(person_request);

            PersonUpdateRequest? person_update_request = person_response_from_add.ToPersonUpdateRequest();

            person_update_request.PersonName = null;

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _personsService.UpdatePerson(person_update_request);
            });
        }

        [Fact]
        public void UpdatePerson_PersonFullDetails()
        {
            //Arrange
            CountryAddRequest country_request = new CountryAddRequest() { CountryName = "Egypt" };
            CountryResponse country_response = _countryService.AddCountry(country_request);

            PersonAddRequest person_request = new PersonAddRequest()
            {
                PersonName = "test",
                Address = "test",
                Email = "test@test.com",
                CountryID = country_response.CountryId,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewLetters = false,
            };

            PersonResponse person_response_from_add = _personsService.AddPerson(person_request);

            PersonUpdateRequest? person_update_request = person_response_from_add.ToPersonUpdateRequest();

            person_update_request.PersonName = "William";

            person_update_request.PersonName = "William@gmail.com";

            //Act
            PersonResponse person_response_from_update = _personsService.UpdatePerson(person_update_request);

            PersonResponse? person_response_from_get = _personsService.GetPersonByPersonID(person_response_from_update.PersonID);

            //Assert
            Assert.Equal(person_response_from_get, person_response_from_update);

        }
        #endregion

        #region DeletePerson
        [Fact]
        public void DeletePerson_NullPersonID()
        {

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                _personsService.DeletePerson(null);
            });
        }

        [Fact]
        public void DeletePerson_InvalidPersonID()
        {
            //Act
            bool isDeleted = _personsService.DeletePerson(Guid.NewGuid());

            //Assert
            Assert.False(isDeleted);
        }

        [Fact]
        public void DeletePerson_ValidPersonID()
        {
            //Arrange
            CountryAddRequest country_request = new CountryAddRequest() { CountryName = "Egypt" };
            CountryResponse country_response = _countryService.AddCountry(country_request);

            PersonAddRequest person_request = new PersonAddRequest()
            {
                PersonName = "test",
                Address = "test",
                Email = "test@test.com",
                CountryID = country_response.CountryId,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewLetters = false,
            };

            PersonResponse person_response_from_add = _personsService.AddPerson(person_request);


            //Act
            bool isDeleted = _personsService.DeletePerson(person_response_from_add.PersonID);

            //Assert
            Assert.True(isDeleted);

        }
        #endregion

    }
}
