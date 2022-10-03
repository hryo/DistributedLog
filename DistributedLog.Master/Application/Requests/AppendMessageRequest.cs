using System.ComponentModel.DataAnnotations;

namespace DistributedLog.Master.Application.Requests;

public class AppendMessageRequest
{
    [Required(AllowEmptyStrings = false)]
    public string Message { get; set; }

    [Required]
    public int ReplicationFactor { get; set; }
}