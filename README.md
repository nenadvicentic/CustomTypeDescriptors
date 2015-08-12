CustomTypeDescriptors
=====================

Several (currently only one) custom TypeDescriptor implementations for .NET Framework.

## Build: ##

Use build.cmd to restore Nuget packages, build project and run unit tests with NUnit.

## Usage: ##
Given that we have `MyModel` class that we want to extend:

    class MyModel
    {
    	[DisplayName("First name")]
    	[Required]
    	public string FirstName { get; set; }
    
    	[StringLength(20, MinimumLength = 5)]
    	public string LastName { get; set; }
    
    	[Range(0, 120)]
    	public int Age { get; set; }
    }

Configure LamdaTypeDescriptor for `MyModel` implicitly:

    // Instantiate configuration
	var config = new LambdaDynamicTypeConfiguration<MyModel>();
	
	// configure metadata
	config.ForProperty(m => m.LastName).
			.RemoveAttribute(typeof(StringLengthAttribute);

	// Add config to provider
	var provider = new LambdaTypeDescriptionProvider(new []{ config });
    TypeDescriptor.AddProvider(provider);




