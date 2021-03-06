using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;

namespace Improbable.Gdk.Core.Commands
{
    public readonly struct CommandContext<T>
    {
        public readonly Entity SendingEntity;
        public readonly T Request;
        public readonly object Context;
        public readonly CommandRequestId RequestId;

        public CommandContext(Entity sendingEntity, T request, object context, CommandRequestId requestId)
        {
            SendingEntity = sendingEntity;
            Request = request;
            Context = context;
            RequestId = requestId;
        }
    }

    public class CommandMetaData
    {
        // Cache the types needed to instantiate a new CommandMetaData
        private static readonly List<(uint componentId, Type command)> StorageTypes;

        private readonly HashSet<InternalCommandRequestId> internalRequestIds = new HashSet<InternalCommandRequestId>();

        private readonly Dictionary<(uint componentId, uint commandId), ICommandMetaDataStorage> componentCommandToStorage =
            new Dictionary<(uint componentId, uint commandId), ICommandMetaDataStorage>();

        static CommandMetaData()
        {
            StorageTypes = ComponentDatabase.Metaclasses
                .SelectMany(type => type.Value.Commands
                    .Select(c => (componentId: type.Value.ComponentId, command: c.MetaDataStorage)))
                .ToList();
        }

        public CommandMetaData()
        {
            foreach (var (componentId, type) in StorageTypes)
            {
                var instance = (ICommandMetaDataStorage) Activator.CreateInstance(type);
                componentCommandToStorage.Add((componentId, instance.CommandId), instance);
            }

            componentCommandToStorage.Add((0, 0), new WorldCommandMetaDataStorage());
        }

        public void RemoveRequest(uint componentId, uint commandId, InternalCommandRequestId internalRequestId)
        {
            var commandMetaDataStorage = GetCommandDiffStorage(componentId, commandId);
            commandMetaDataStorage.RemoveMetaData(internalRequestId);
            internalRequestIds.Remove(internalRequestId);
        }

        public void AddRequest<T>(uint componentId, uint commandId, in CommandContext<T> context)
        {
            var commandPayloadStorage = (ICommandPayloadStorage<T>) GetCommandDiffStorage(componentId, commandId);
            commandPayloadStorage.AddRequest(in context);
        }

        public void AddInternalRequestId(uint componentId, uint commandId, CommandRequestId requestId, InternalCommandRequestId internalRequestId)
        {
            internalRequestIds.Add(internalRequestId);
            var commandMetaDataStorage = GetCommandDiffStorage(componentId, commandId);
            commandMetaDataStorage.SetInternalRequestId(internalRequestId, requestId);
        }

        public CommandContext<T> GetContext<T>(uint componentId, uint commandId, InternalCommandRequestId internalRequestId)
        {
            var commandPayloadStorage = (ICommandPayloadStorage<T>) GetCommandDiffStorage(componentId, commandId);
            return commandPayloadStorage.GetPayload(internalRequestId);
        }

        private ICommandMetaDataStorage GetCommandDiffStorage(uint componentId, uint commandId)
        {
            if (!componentCommandToStorage.TryGetValue((componentId, commandId), out var storage))
            {
                throw new ArgumentException($"Can not find command metadata. Unknown command ID {commandId} on component {componentId}.");
            }

            return storage;
        }
    }
}
