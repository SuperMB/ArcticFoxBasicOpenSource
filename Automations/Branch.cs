namespace ArcticFoxBasic.Automations;

public class Branch : VerilogAutomation
{
    // Because we want to be adaptable to any design, must have a clock 
    // defined for the module
    protected override Dependencies Dependencies => Module.PrimaryClockSet;

    protected override void ApplyAutomation()
    {
        // Get the branch count from Items, otherwise throw an error
        int branchCount = Items[0, "Count"] | -1;
        if(branchCount == -1)
            throw new Exception("Branch Automation Error: The branch count was not given as an item.");

        // Get the next variable, aka, either wire or reg
        IVariable variable = Module.NextVariable(this);

        // Get the clock name
        string clockName = Module.PrimaryClock.Name;

        // Add code to the start of the module to define each of the regs
        // that will be used to branch the signal
        CodeModuleStart += $@"
    {(1, branchCount).For( i => $@"
        reg {variable.WidthHDLTokenStream} {variable.Name}_{i};")}
";

        // Add code to the end of the module to 
        CodeModuleEnd += $@"
    {(1, branchCount).For( i => $@"
        always@(posedge {clockName}) begin
            {IfReset($"{variable.Name}_{i} <= 0;")}
            {variable.Name}_{i} <= {variable.Name};
        end")}
";

    }

}
