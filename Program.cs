using System;

// Main program class that serves as the entry point
class Program
{
    static void Main(string[] args)
    {
        // Create and start the shipping quote application
        var shippingApp = new ShippingQuoteApplication();
        shippingApp.Start();
    }
}

// Enum defining different states of the shipping quote process
enum ShippingQuoteState
{
    Start,
    WeightInput,
    DimensionsInput,
    QuoteCalculation,
    Complete,
    Error
}

// Class responsible for managing the shipping quote application flow
class ShippingQuoteApplication
{
    // State management variables
    private ShippingQuoteState _currentState;
    private readonly PackageInfo _package;
    private readonly ShippingCalculator _calculator;
    private readonly InputValidator _validator;

    // Constructor initializes components
    public ShippingQuoteApplication()
    {
        _currentState = ShippingQuoteState.Start;
        _package = new PackageInfo();
        _calculator = new ShippingCalculator();
        _validator = new InputValidator();
    }

    // Method to start the application
    public void Start()
    {
        // Display welcome message
        Console.WriteLine("Welcome to Package Express. Please follow the instructions below.");
        ProcessState();
    }

    // Main state processing loop
    private void ProcessState()
    {
        while (_currentState != ShippingQuoteState.Complete && 
               _currentState != ShippingQuoteState.Error)
        {
            switch (_currentState)
            {
                case ShippingQuoteState.Start:
                    _currentState = ShippingQuoteState.WeightInput;
                    break;

                case ShippingQuoteState.WeightInput:
                    ProcessWeightInput();
                    break;

                case ShippingQuoteState.DimensionsInput:
                    ProcessDimensionsInput();
                    break;

                case ShippingQuoteState.QuoteCalculation:
                    CalculateAndDisplayQuote();
                    break;
            }
        }
    }

    // Process weight input from user
    private void ProcessWeightInput()
    {
        try
        {
            Console.WriteLine("Please enter the package weight:");
            _package.Weight = Convert.ToDouble(Console.ReadLine());

            if (!_validator.ValidateWeight(_package.Weight))
            {
                Console.WriteLine("Package too heavy to be shipped via Package Express. Have a good day.");
                _currentState = ShippingQuoteState.Error;
                return;
            }

            _currentState = ShippingQuoteState.DimensionsInput;
        }
        catch (Exception)
        {
            Console.WriteLine("Invalid input. Please enter a valid number.");
        }
    }

    // Process dimensions input from user
    private void ProcessDimensionsInput()
    {
        try
        {
            // Get width
            Console.WriteLine("Please enter the package width:");
            _package.Width = Convert.ToDouble(Console.ReadLine());

            // Get height
            Console.WriteLine("Please enter the package height:");
            _package.Height = Convert.ToDouble(Console.ReadLine());

            // Get length
            Console.WriteLine("Please enter the package length:");
            _package.Length = Convert.ToDouble(Console.ReadLine());

            if (!_validator.ValidateDimensions(_package))
            {
                Console.WriteLine("Package too big to be shipped via Package Express.");
                _currentState = ShippingQuoteState.Error;
                return;
            }

            _currentState = ShippingQuoteState.QuoteCalculation;
        }
        catch (Exception)
        {
            Console.WriteLine("Invalid input. Please enter valid numbers.");
        }
    }

    // Calculate and display the shipping quote
    private void CalculateAndDisplayQuote()
    {
        double quote = _calculator.CalculateQuote(_package);
        Console.WriteLine($"Your estimated total for shipping this package is: ${quote:F2}");
        Console.WriteLine("Thank you!");
        _currentState = ShippingQuoteState.Complete;
    }
}

// Class representing package information
class PackageInfo
{
    public double Weight { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Length { get; set; }

    // Calculate total dimensions
    public double TotalDimensions => Width + Height + Length;
}

// Class handling shipping calculations
class ShippingCalculator
{
    // Calculate shipping quote based on package dimensions and weight
    public double CalculateQuote(PackageInfo package)
    {
        return (package.Width * package.Height * package.Length * package.Weight) / 100;
    }
}

// Class handling input validation
class InputValidator
{
    // Constants for validation
    private const int MaxWeight = 50;
    private const int MaxTotalDimensions = 50;

    // Validate package weight
    public bool ValidateWeight(double weight)
    {
        return weight <= MaxWeight;
    }

    // Validate package dimensions
    public bool ValidateDimensions(PackageInfo package)
    {
        return package.TotalDimensions <= MaxTotalDimensions;
    }
}