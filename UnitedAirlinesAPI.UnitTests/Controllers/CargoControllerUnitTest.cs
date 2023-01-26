using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using System.ComponentModel.DataAnnotations;
using UnitedAirlinesAPI.Controllers;
using UnitedAirlinesAPI.Infrastructure;
using UnitedAirlinesAPI.Models;
using UnitedAirlinesAPI.Services;

namespace UnitedAirlinesAPI.UnitTests.Controllers
{
    public class CargoControllerUnitTest : UnitTestBase
    {
        private CargoController _cargoController;
        private Mock<ICargoService> _mockCargoService;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _mockCargoService = new Mock<ICargoService>();
            _cargoController = new CargoController(_mockCargoService.Object);
        }

        [Test]
        public void GetCargoInfoAsync_ShouldThrowError_WhenModelIsInvalid()
        {
            //Arrange
            //create a model request with valid properties at first
            var model = new CargoRequest()
            {
                AirportCode = "PDX",
                FlightDate = "AS-7095-01Jan2023"
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            //Act
            var actual = Validator.TryValidateObject(model, context, results);
            var result = _cargoController.GetCargoInfoAsync(model);

            //Assert
            Assert.True(actual, "Expects validation to pass");
            //now make model property invalid
            model.AirportCode = "asdfsdf";
            results.Clear();
            actual = Validator.TryValidateObject(model, context, results, validateAllProperties: true);
            Assert.True(actual, "Expects validation to fail"); // Fails here
        }
    }
}