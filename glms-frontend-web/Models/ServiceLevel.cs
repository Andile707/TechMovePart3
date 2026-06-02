using System.ComponentModel.DataAnnotations;

namespace glms_frontend_web.Models
{
    public enum ServiceLevel
    {
        [Display(Name = "Local Delivery")]
        LocalDelivery,

        [Display(Name = "Express Delivery")]
        ExpressDelivery,

        [Display(Name = "Premium Logistics")]
        PremiumLogistics,

        [Display(Name = "Enterprise Logistics")]
        EnterpriseLogistics
    }
}