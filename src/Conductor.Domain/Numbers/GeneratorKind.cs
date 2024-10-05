using Ardalis.SmartEnum;

namespace Conductor.Domain.Numbers;

// Dont use word "Type"
public class GeneratorKind : SmartEnum<GeneratorKind>
{
    public static readonly GeneratorKind General = new(nameof(General), 1);

    public static readonly GeneratorKind Process = new(nameof(Process), 2);

    public static readonly GeneratorKind Deployment = new(nameof(Deployment), 3);

    private GeneratorKind(string name, int value) : base(name, value)
    {
    }

    public bool IsGeneral => this == General;

    public bool IsProcess => this == Process;

    public bool IsDeployment => this == Deployment;
}
