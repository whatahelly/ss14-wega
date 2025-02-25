using System.IO;
using System.Linq;
using Content.Shared.Chat.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace Content.Shared.Interaction.Panel;

public sealed class InteractionPanelManager : IPostInjectInit
{
    [Dependency] private readonly ISerializationManager _serManager = default!;
    private ISawmill _sawmill = default!;

    public void PostInject()
    {
        // Empty
    }

    /// <summary>
    /// !@#%*#%*#$!#@$
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException">:P</exception>
    public List<InteractionPrototype> FromStream(Stream stream)
    {
        try
        {
            using var reader = new StreamReader(stream, EncodingHelpers.UTF8);
            var yamlStream = new YamlStream();
            yamlStream.Load(reader);

            if (yamlStream.Documents.Count == 0)
            {
                throw new InvalidDataException("YAML file is empty.");
            }

            var root = yamlStream.Documents[0].RootNode;
            var prototypes = _serManager.Read<List<InteractionPrototype>>(root.ToDataNode(), notNullableOverride: true);

            if (prototypes == null || prototypes.Count == 0)
            {
                throw new InvalidDataException("No valid prototypes found in the file.");
            }

            return prototypes;
        }
        catch (Exception ex)
        {
            _sawmill = Logger.GetSawmill("interaction_import");
            _sawmill.Error($"Error while reading YAML file: {ex.Message}");
            throw;
        }
    }

    public DataNode ToDataNode(InteractionPrototype prototype)
    {
        var sequenceNode = new SequenceDataNode();
        var mapping = new MappingDataNode();

        mapping.Add("type", new ValueDataNode("interaction"));
        mapping.Add("id", new ValueDataNode(prototype.ID));
        mapping.Add("name", new ValueDataNode(prototype.Name));
        mapping.Add("erp", new ValueDataNode(prototype.ERP.ToString()));

        if (prototype.Icon != null)
        {
            mapping.Add("icon", new ValueDataNode(prototype.Icon.ToString()));
        }

        if (prototype.UserMessages != null && prototype.UserMessages.Count > 0)
        {
            var userMessagesNode = new SequenceDataNode();
            foreach (var message in prototype.UserMessages)
            {
                userMessagesNode.Add(new ValueDataNode(message));
            }
            mapping.Add("userMessages", userMessagesNode);
        }

        if (prototype.AllowedSpecies != null && prototype.AllowedSpecies.Count > 0)
        {
            var allowedSpeciesNode = new SequenceDataNode();
            foreach (var species in prototype.AllowedSpecies)
            {
                allowedSpeciesNode.Add(new ValueDataNode(species));
            }
            mapping.Add("allowedSpecies", allowedSpeciesNode);
        }

        if (prototype.AllowedGenders != null && prototype.AllowedGenders.Count > 0)
        {
            var allowedGendersNode = new SequenceDataNode();
            foreach (var gender in prototype.AllowedGenders)
            {
                allowedGendersNode.Add(new ValueDataNode(gender));
            }
            mapping.Add("allowedGenders", allowedGendersNode);
        }

        if (prototype.NearestAllowedSpecies != null && prototype.NearestAllowedSpecies.Count > 0)
        {
            var nearestAllowedSpeciesNode = new SequenceDataNode();
            foreach (var species in prototype.NearestAllowedSpecies)
            {
                nearestAllowedSpeciesNode.Add(new ValueDataNode(species));
            }
            mapping.Add("nearestAllowedSpecies", nearestAllowedSpeciesNode);
        }

        if (prototype.NearestAllowedGenders != null && prototype.NearestAllowedGenders.Count > 0)
        {
            var nearestAllowedGendersNode = new SequenceDataNode();
            foreach (var gender in prototype.NearestAllowedGenders)
            {
                nearestAllowedGendersNode.Add(new ValueDataNode(gender));
            }
            mapping.Add("nearestAllowedGenders", nearestAllowedGendersNode);
        }

        if (prototype.BlackListSpecies != null && prototype.BlackListSpecies.Count > 0)
        {
            var blackListSpeciesNode = new SequenceDataNode();
            foreach (var species in prototype.BlackListSpecies)
            {
                blackListSpeciesNode.Add(new ValueDataNode(species));
            }
            mapping.Add("blackListSpecies", blackListSpeciesNode);
        }

        if (prototype.InteractSound != null)
        {
            var soundNode = new MappingDataNode();
            if (prototype.InteractSound is SoundCollectionSpecifier collectionSpecifier)
            {
                soundNode.Add("collection", new ValueDataNode(collectionSpecifier.Collection));
            }
            else if (prototype.InteractSound is SoundPathSpecifier pathSpecifier)
            {
                soundNode.Add("path", new ValueDataNode(pathSpecifier.Path.ToString()));
            }
            mapping.Add("interactSound", soundNode);
        }

        mapping.Add("soundPerceivedByOthers", new ValueDataNode(prototype.SoundPerceivedByOthers.ToString()));

        sequenceNode.Add(mapping);

        return sequenceNode;
    }
}
