namespace River.API.DTOs.Transfer;


public class SimulateResponseDto {

    public required string From { get; set; }
    public required string To { get; set; }
    public required decimal FromBalanceAfter { get; set; }
    public required decimal ToBalanceAfter { get; set; }
    public required decimal FromBalanceBefore { get; set; }
    public required decimal ToBalanceBefore { get; set; }


}