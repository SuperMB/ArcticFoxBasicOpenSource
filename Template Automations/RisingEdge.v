@Description
// Automation Name: RisingEdge

@AfterNext
//[Previous]
reg p_{NextVariable().Name};

@ModuleEnd
assign rising{NextVariable().Name} = {NextVariable().Name} && !p_{NextVariable().Name};