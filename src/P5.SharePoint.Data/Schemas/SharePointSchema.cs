using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using P5.SharePoint.Core.Models.Common;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;
using GraphQL;
using GraphQL.Builders;
using P5.SharePoint.Data.Queries;
using GraphQL.Resolvers;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Authorization;

namespace P5.SharePoint.Data.Schemas
{
    public class SharePointSchema : ISchemaBuilder
    {
        private readonly IMediator _mediator;
        private readonly Func<SignInManager<ApplicationUser>> _signInManagerFactory;
        public SharePointSchema(
            IMediator mediator,
            IAuthorizationService authorizationService,
            Func<SignInManager<ApplicationUser>> signInManagerFactory
      
        )
        {
            _mediator =  mediator;
            _signInManagerFactory = signInManagerFactory;
        }
            
        public void Build(ISchema schema)
        {

            _ = schema.Query.AddField(new FieldType
            {
                Name = "sharepointTitle",
                Arguments = new QueryArguments(),
                Type = GraphTypeExtenstionHelper.GetActualType<SharePointTitleResponseType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var query = new GetSharepointSiteTitleQuery();
                    var result = await _mediator.Send(query);

                    return result;
                })
            });

            _ = schema.Query.AddField(new FieldType
            {
                Name = "sharepointLists",
                Arguments = new QueryArguments(),
                Type = GraphTypeExtenstionHelper.GetActualType<SharePointListsResponseType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var query = new GetSharepointListsQuery();
                    var result = await _mediator.Send(query);
                    return result;
                })
            });

            _ = schema.Query.AddField(new FieldType
            {
                Name = "sharepointLibrary",
                Arguments = new QueryArguments(),
                Type = GraphTypeExtenstionHelper.GetActualType<SharePointLibraryResponseType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var query = new GetSharepointLibrariesQuery();
                    var result = await _mediator.Send(query);
                    return result;
                })
            });


            _ = schema.Query.AddField(new FieldType
            {
                Name = "sharepointFileById",
                Arguments = new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "id" }),
                Type = GraphTypeExtenstionHelper.GetActualType<SharePointFileIdResponseType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var query = new GetSharepointFileByIdQuery(context.GetArgument<Guid>("id"));
                    var result = await _mediator.Send(query);
                    return result;
                })  
            });

            _ = schema.Query.AddField(new FieldType
            {
                Name = "GetSharePointFileDetail",
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id" }),
                Type = GraphTypeExtenstionHelper.GetActualType<SharePointSearchResponseType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var query = new SearchSharePointQuery(context.GetArgument<string>("id"));
                    var result = await _mediator.Send(query);
                    return result;
                })
            });
        }
    }
}
