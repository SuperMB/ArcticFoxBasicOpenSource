namespace ArcticFoxBasic.Automations;

public class RisingEdge : VerilogAutomation
{
    protected override Dependencies Dependencies => Dependencies.None;

	protected override void ApplyAutomation()
	{

		// Get the next wire, force it to a wire if it is a reg
		Wire risingWire = NextWire(forceToWire: true);

		// Get the signal to detect the rise from if explicitly stated
		string source = Items[0, "Signal"];
		if(source == Item.Missing)
		{
			// If not explicitly stated, see if it can be infered from the attached wire
			// starting with the name rising
			if(risingWire.Name.Length > 6 && risingWire.Name.Substring(0,6) == "rising")
			{
				source = risingWire.Name.Substring(6);
				source = char.ToLower(source[0]) + source.Substring(1);
			}

			// If the signal to detect the rising edge of is not explicitly stated, and 
			// cannot be infered, throw an error
			else 
                throw new Exception("RisingEdge Automation Error: Not given an appropriate name or convention for the rising edge signal to detect.");
		}

		// Generate code at the end of the module to detect the rising of the desired signal
		// One of the biggest things to note is the use of the Previous automation, by including
		// the //[Previous {source}] in the code, it will call into the Previous automation
		CodeModuleEnd += @$"
//[Previous {source}]
reg p1_{source};
assign {risingWire.Name} = {source} && !p1_{source};";
	}

}