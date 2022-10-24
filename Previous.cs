using System.Text.RegularExpressions;

namespace ArcticFoxBasic.Automations;

public class Previous : VerilogAutomation
{
    // Because we want this to be adaptable to any users code, we want the
    // clock name in the always block to be that of the primary clock set
    // for the module to 
    protected override Dependencies Dependencies => Module.PrimaryClockSet;

    protected override void ApplyAutomation()
    {
        // Get the next Reg following the Previous automation,
        // if it is a "wire" force it to become a "reg"
        Reg reg = Module.NextReg(this, forceToReg: true);

        // If the Previous automation is not followed by a
        // reg or wire, throw an error
        if (reg == null)
            throw NotFollowedByReg();

        // Get the name of the attached reg
        string regName = reg.Name;
        
        // Get the name of the primary clock for the module
        string clockName = Module.PrimaryClock.Name;

        // Check and see if an explicit signal to follow is presented
        string signalToFollow = Items[0, "Signal"];
        int index = -1;
        if(signalToFollow == Item.Missing)
        {
            // If the signal to follow is not explicitly stated,
            // try to infer it if the regName starts with as p#_
            if(regName.Length > 3 && Regex.IsMatch(regName, @"^p\d*_"))
            {
                Match match = Regex.Match(regName, @"^p(\d*)_(.+)");
                signalToFollow = match.Groups[2].Value;

                if(int.TryParse(match.Groups[1].Value, out int value))
                    index = value;
            }

            // If the regName did not start with p#_ and the signal was not 
            // explicitly given, then we do not have a signal to follow, 
            // throw an error
            else 
                throw new Exception("Previous Automation Error: Not given an appropriate name or convention for the previous signal to follow.");
        }
            

        // Generate the code at the end of the module such that regName is set
        // to the value of the signal it follows at the rising edge of the clock 
        CodeModuleEnd += $@"
always@(posedge {clockName}) begin
    {IfReset($"{regName} <= 0;")}
    {regName} <= {(index > 1 ? $"p{index - 1}_" : "")}{signalToFollow};
end";
    }

    // Error to generate if the Previous automation is not attached to a reg or wire
    private Exception NotFollowedByReg()
    {
        string errorCode = "";

        // Get the Start of the Attribute HDL tokens
        HDLTokenNode node = HDLTokenStream.Start;

        // For the error, so that we know where we are in the code, get
        // the 50 next HDL tokens
        for(int i = 0; i < 50; i++)
        {
            errorCode += node;
            node = node.NextWithWhitespace();
        }

        // Create the error to throw
        return new Exception($"Previous Automation Error: Module: {Module.ModuleName} - Next reg could not be found.\nCode: {errorCode}");
    }
}
