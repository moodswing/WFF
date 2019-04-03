using AutoMapper;
using WFF.Models;
using WFF.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace WFF.Utils
{
    public static class MappingConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(x =>
            {
                x.CreateMap<FormRequest, FormRequestViewModel>();
                x.CreateMap<Attachment, FieldAttachmentViewModel>()
                    .ForMember(d => d.ControlId, d => d.MapFrom(y => y.Field));
                x.CreateMap< List <Attachment>, List<FieldAttachmentViewModel>> ()
                    .ConvertUsing<HttpPostedFileBaseTypeConverter>();
            });
        }
    }

    public class HttpPostedFileBaseTypeConverter :ITypeConverter<List<Attachment>, List<FieldAttachmentViewModel>>
    {
        public List<FieldAttachmentViewModel> Convert(List<Attachment> source, List<FieldAttachmentViewModel> destination, ResolutionContext context)
        {
            var viewModelList = new List<FieldAttachmentViewModel>();
            if (source == null) return viewModelList;

            var fields = source.GroupBy(e => e.Field, (key, g) => new { Field = key, Files = g.ToList() });
            foreach (var field in fields)
            {
                var viewModel = new FieldAttachmentViewModel();
                viewModel.ControlId = field.Field;
                viewModel.Files = new List<AttachmentFile>();

                foreach (var file in field.Files)
                    viewModel.Files.Add(
                        new AttachmentFile
                        {
                            Id = file.ID,
                            FileName = file.FileName,
                            Location = file.Location
                        }
                    );

                viewModelList.Add(viewModel);
            }

            return viewModelList;
        }
    }
}