using System.ComponentModel.DataAnnotations;

namespace HaberPortal.ViewModels
{
    public class NewsModel
    {
        public int Id { get; set; }


        [Display(Name = "Haber Başlığı")]

        [Required(ErrorMessage = "Lütfen Haber Başlığı Giriniz!")]
        public string Title { get; set; }


        [Display(Name = "Haber Açıklaması")]

        [Required(ErrorMessage = "Lütfen Haber Açıklaması Giriniz!")]
        public string Description { get; set; }


        [Display(Name = "Haber İçeriği")]

        [Required(ErrorMessage = "Lütfen Haber İçeriği Giriniz!")]
        public string Content { get; set; }


        [Display(Name = "Haber Etiketi")]

        [Required(ErrorMessage = "Lütfen Haber Etiketi Giriniz!")]
        public string Tag { get; set; }


        [Display(Name = "Haber Fotoğrafı")]

        [Required(ErrorMessage = "Lütfen Haber Fotoğrafı Giriniz!")]
        public IFormFile PhotoFile { get; set; }
    }
}
