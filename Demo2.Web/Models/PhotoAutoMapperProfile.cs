using Demo2.Data.Entities;

namespace Demo2.Web.Models
{
    public class PhotoAutoMapperProfile : AutoMapper.Profile
    {
        public PhotoAutoMapperProfile()
        {
            CreateMap<Photo, PhotoModel>()
                .AfterMap<PhotoMapingAction>();

            CreateMap<Photo, EditPhotoModel>();
        }
    }
}