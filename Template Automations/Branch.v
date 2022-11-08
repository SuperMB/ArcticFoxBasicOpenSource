@Description
// Automation Name: Branch

@ModuleStart
{(1, Items[0].ToInt()).For( i => $@"
reg {NextVariable().Width} {NextVariable().Name}_{i};")}

@ModuleEnd
{(1, Items[0].ToInt()).For( i => $@"
always@(posedge clk) begin
    {IfReset($"{NextVariable().Name}_{i} <= 0;")}
    {NextVariable().Name}_{i} <= {NextVariable().Name};
end")} 