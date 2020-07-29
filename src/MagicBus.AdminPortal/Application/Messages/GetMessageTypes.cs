using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicBus.AdminPortal.Application.Models;
using MagicBus.Messages.Common;
using MediatR;

namespace MagicBus.AdminPortal.Application.Messages
{
    public class GetMessageTypes: IRequest<IEnumerable<MessageTypeDto>>
    {
    }
    
    public class GetMessageTypesHandler: IRequestHandler<GetMessageTypes, IEnumerable<MessageTypeDto>>
    {
        public Task<IEnumerable<MessageTypeDto>> Handle(GetMessageTypes request, CancellationToken cancellationToken)
        {

            var messageBaseType = (typeof(MessageBase));
            var messageTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes().Where(p => messageBaseType.IsAssignableFrom(p)))
                .ToList();

            var result = messageTypes.Select(t => new MessageTypeDto()
                {
                    Name = t.Name,
                    FullName = t.AssemblyQualifiedName
                });

            return Task.FromResult(result);
        }
    }
}