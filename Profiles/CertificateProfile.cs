using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CertificatesManager.Models;
using CertificatesManager.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CertificatesManager.Profiles
{
	public class CertificateProfile : Profile
	{
		public CertificateProfile()
		{
			CreateMap<Certificate, CertificateViewModel>();
			CreateMap<CertificateViewModel, Certificate>();
			CreateMap<CertificateApplication, SelectListItem>()
				.ForMember(
					dest => dest.Value, opt => opt.MapFrom(src => src.ApplicationId.ToString())
					)
				.ForMember(
					dest => dest.Text, opt => opt.MapFrom(src => src.Application.Name)
					);
			CreateMap<CertificateServer, SelectListItem>()
				.ForMember(
					dest => dest.Value, opt => opt.MapFrom(src => src.ServerId.ToString())
					)
				.ForMember(
					dest => dest.Text, opt => opt.MapFrom(src => src.Server.Name)
					);
			CreateMap<CertificateGroup, SelectListItem>()
				.ForMember(
					dest => dest.Value, opt => opt.MapFrom(src => src.GroupId.ToString())
					)
				.ForMember(
					dest => dest.Text, opt => opt.MapFrom(src => src.Group.Name)
					).ForAllOtherMembers(x => x.Ignore());

			CreateMap<Group, GroupViewModel>();
			CreateMap<GroupViewModel, Group>();
			CreateMap<GroupUser, SelectListItem>()
				.ForMember(
						dest => dest.Value, opt => opt.MapFrom(src => src.UserId.ToString())
					)
				.ForMember(
						dest => dest.Text, opt => opt.MapFrom(src => src.User.Name)
				).ForAllOtherMembers(x => x.Ignore());

			CreateMap<Settings, SettingsViewModel>()
				.ForMember(
				dest => dest.Group, opt => opt.MapFrom(src => src.Group.Name));
			CreateMap<SettingsViewModel, Settings>().ForMember(
				dest => dest.Group, opt => opt.MapFrom(src => new Group() {Id = Convert.ToInt32(src.Group) }));
			CreateMap<Settings, SelectListItem>()
				.ForMember(
						dest => dest.Value, opt => opt.MapFrom(src => src.Group.Id.ToString())
						)
						.ForMember(
						dest => dest.Text, opt => opt.MapFrom(src => src.Group.Name)
					).ForAllOtherMembers(x => x.Ignore());
		}
	}
}
