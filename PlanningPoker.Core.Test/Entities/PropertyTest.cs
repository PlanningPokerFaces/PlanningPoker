using PlanningPoker.Core.Entities;

namespace PlanningPoker.Core.Test.Entities;

[TestFixture]
[TestOf(typeof(Property))]
[Category("Core")]
public class PropertyTest
{
    [Test]
    public void CreateLabelProperty_FieldsCanBeRetrieved()
    {
        // Arrange
        var dictionary = new Dictionary<string, string> { { "Name", "Scrum: SP 5" } };

        // Act
        var property = new Property { Type = PropertyType.Label, Data = dictionary };

        // Assert
        var resultType = property.Type;
        var resultDictionary = property.Data;
        Assert.Multiple(() =>
        {
            Assert.That(resultType, Is.EqualTo(PropertyType.Label));
            Assert.That(resultDictionary, Has.Count.EqualTo(1));
            Assert.That(resultDictionary, Is.EqualTo(dictionary));
        });
    }

    [Test]
    public void CreateLabelProperty_WithEmptyDictionary_Throws()
    {
        // Act
        TestDelegate illegalAction = () =>
        {
            _ = new Property { Type = PropertyType.Label, Data = new Dictionary<string, string>() };
        };

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(illegalAction);
    }

    [Test]
    public void PropertiesWithDifferentDataCounts_AreNotEqual()
    {
        // Arrange
        var data1 = new Dictionary<string, string> { { "Name", "Scrum: SP 5" } };
        var data2 = new Dictionary<string, string> { { "Name", "Scrum: SP 5" }, { "Description", "Estimation" } };
        var property1 = new Property { Type = PropertyType.Label, Data = data1 };
        var property2 = new Property { Type = PropertyType.Label, Data = data2 };

        // Act & Assert
        Assert.That(property1.IsEqualProperty(property2), Is.False);
    }

    [Test]
    public void PropertiesWithSameTypeAndData_AreEqual()
    {
        // Arrange
        var data1 = new Dictionary<string, string> { { "Name", "Scrum: SP 5" } };
        var data2 = new Dictionary<string, string> { { "Name", "Scrum: SP 5" } };
        var property1 = new Property { Type = PropertyType.Label, Data = data1 };
        var property2 = new Property { Type = PropertyType.Label, Data = data2 };

        // Act & Assert
        Assert.That(property1.IsEqualProperty(property2), Is.True);
    }

    [Test]
    public void PropertiesWithDifferentTypes_AreNotEqual()
    {
        // Arrange
        var data = new Dictionary<string, string> { { "Name", "Scrum: SP 5" } };
        var property1 = new Property { Type = PropertyType.Label, Data = data };
        var property2 = new Property { Type = PropertyType.Undefined, Data = data };

        // Act & Assert
        Assert.That(property1.IsEqualProperty(property2), Is.False);
    }

    [Test]
    public void PropertiesWithDifferentData_AreNotEqual()
    {
        // Arrange
        var data1 = new Dictionary<string, string> { { "Name", "Scrum: SP 5" } };
        var data2 = new Dictionary<string, string> { { "Name", "Scrum: tb 4h" } };
        var property1 = new Property { Type = PropertyType.Label, Data = data1 };
        var property2 = new Property { Type = PropertyType.Label, Data = data2 };

        // Act & Assert
        Assert.That(property1.IsEqualProperty(property2), Is.False);
    }

    [Test]
    public void PropertyComparedWithNull_IsNotEqual()
    {
        // Arrange
        var data = new Dictionary<string, string> { { "Name", "Scrum: SP 5" } };
        var property = new Property { Type = PropertyType.Label, Data = data };

        // Act & Assert
        Assert.That(property.IsEqualProperty(null), Is.False);
    }

    [Test]
    public void PropertyIsEqualToItself()
    {
        // Arrange
        var data = new Dictionary<string, string> { { "Name", "Scrum: SP 5" } };
        var property = new Property { Type = PropertyType.Label, Data = data };

        // Act & Assert
        Assert.That(property.IsEqualProperty(property), Is.True);
    }
}

