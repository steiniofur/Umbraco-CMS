using System.Runtime.Serialization;

namespace Umbraco.Cms.Core.Install.Models
{
    [DataContract(Name = "user", Namespace = "")]
    public class UserModel
    {
        [DataMember(Name = "name")]
        public required string Name { get; set; }

        [DataMember(Name = "email")]
        public required string Email { get; set; }

        [DataMember(Name = "password")]
        public required string Password { get; set; }

        [DataMember(Name = "subscribeToNewsLetter")]
        public bool SubscribeToNewsLetter { get; set; }
    }
}
