@Description
// Automation Name: Previous

@AfterAutomation
reg {NextReg().Width} p_{NextReg().Name};

@ModuleEnd
always@(posedge clk) begin
    if(reset)
        p_{NextReg().Name} <= 0;
    else 
        p_{NextReg().Name} <= {NextReg().Name};
end