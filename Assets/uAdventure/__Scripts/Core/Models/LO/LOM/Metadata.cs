using System;
using System.Xml.Serialization;
using UnityEngine;

namespace uAdventure.Core.Metadata
{
    [XmlType(Namespace = "http://www.imsglobal.org/xsd/imsmd_v1p2")]
    public class Metadata : ScriptableObject
    {
        [SerializeField]
        private General general;
        [SerializeField]
        private LifeCycle lifeCycle;
        [SerializeField]
        private MetaMetaData metaMetaData;
        [SerializeField]
        private Technical technical;
        [SerializeField]
        private Educational[] educational;
        [SerializeField]
        private Rights rights;
        [SerializeField]
        private Relation[] relation;
        [SerializeField]
        private Annotation[] annotation;
        [SerializeField]
        private Classification[] classification;

        [XmlElement("general")]
        public General General { get => general; set => general = value; }
        [XmlElement("lifeCycle")]
        public LifeCycle LifeCycle { get => lifeCycle; set => lifeCycle = value; }
        [XmlElement("metaMetaData")]
        public MetaMetaData MetaMetaData { get => metaMetaData; set => metaMetaData = value; }
        [XmlElement("technical")]
        public Technical Technical { get => technical; set => technical = value; }
        [XmlElement("educational")]
        public Educational[] Educational { get => educational; set => educational = value; }
        [XmlElement("rights")]
        public Rights Rights { get => rights; set => rights = value; }
        [XmlElement("relation")]
        public Relation[] Relation { get => relation; set => relation = value; }
        [XmlElement("annotation")]
        public Annotation[] Annotation { get => annotation; set => annotation = value; }
        [XmlElement("classification")]
        public Classification[] Classification { get => classification; set => classification = value; }
    }

    #region General

    [Serializable]
    public class General
    {
        [SerializeField]
        private Identifier[] identifier;
        [SerializeField]
        private LangString title;
        [SerializeField]
        private string[] language;
        [SerializeField]
        private LangString[] description;
        [SerializeField]
        private LangString[] keyword;
        [SerializeField]
        private LangString[] coverage;
        [SerializeField]
        [LomTypeEnum("LOMv1.0", typeof(Lom1StructureEnum))]
        private Structure structure;
        [SerializeField]
        [LomTypeEnum("LOMv1.0", typeof(Lom1AggregationLevelEnum))]
        private AggregationLevel aggregationLevel;

        [XmlElement("identifier")]
        public Identifier[] Identifier { get => identifier; set => identifier = value; }
        [XmlElement("title")]
        public LangString Title { get => title; set => title = value; }
        public string[] Language { get => language; set => language = value; }
        public LangString[] Description { get => description; set => description = value; }
        public LangString[] Keyword { get => keyword; set => keyword = value; }
        public LangString[] Coverage { get => coverage; set => coverage = value; }
        public Structure Structure { get => structure; set => structure = value; }
        public AggregationLevel AggregationLevel { get => aggregationLevel; set => aggregationLevel = value; }
    }

    [Serializable]
    public class LangString
    {
        [SerializeField]
        private LanguageString[] @string;

        [Serializable]
        public class LanguageString
        {
            [SerializeField]
            private string value;
            [SerializeField]
            private string language;

            public string Value { get => value; set => this.value = value; }
            public string Language { get => language; set => language = value; }
        }

        public LangString()
        {
            @string = new LanguageString[1] { new LanguageString() };
        }

        public LanguageString[] String { get => @string; set => @string = value; }

    }

    [Serializable]
    public class LomType
    {
        [SerializeField]
        private string source;
        [SerializeField]
        private string value;

        public virtual string Source { get => source; set => source = value; }

        public virtual string Value { get => value; set => this.value = value; }
    }

    [Serializable]
    public class Structure : LomType { }

    public enum Lom1StructureEnum
    {
        None, Atomic, Collection, Networked, Hierarchical, Linear
    }

    [Serializable]
    public class Lom1Structure : Structure
    {
        public enum Values
        {
            None, Atomic, Collection, Networked, Hierarchical, Linear
        }

        public Values EnumValue { get; set; } = Values.None;

        public override string Source { get { return "LOMv1.0"; } set { } }

        public override string Value
        {
            get
            {
                switch (EnumValue)
                {
                    case Values.Atomic:
                        return "atomic";
                    case Values.Collection:
                        return "collection";
                    case Values.Networked:
                        return "networked";
                    case Values.Hierarchical:
                        return "hierarchical";
                    case Values.Linear:
                        return "linear";
                    default:
                        return "";

                }
            }
            set { }
        }
    }
    [Serializable]
    public class Identifier
    {
        [SerializeField]
        private string catalog;
        [SerializeField]
        private string entry;

        public string Catalog { get => catalog; set => catalog = value; }
        public string Entry { get => entry; set => entry = value; }
    }
    [Serializable]
    public class AggregationLevel : LomType { }


    public enum Lom1AggregationLevelEnum
    {
        None, _1, _2, _3, _4
    }
    [Serializable]
    public class Lom1AggregationLevel : AggregationLevel
    {
        public enum Values
        {
            None, Value1, Value2, Value3, Value4
        }

        public Values EnumValue { get; set; } = Values.None;

        public override string Source { get { return "LOMv1.0"; } set { } }

        public override string Value
        {
            get
            {
                switch (EnumValue)
                {
                    case Values.Value1:
                        return "1";
                    case Values.Value2:
                        return "2";
                    case Values.Value3:
                        return "3";
                    case Values.Value4:
                        return "4";
                    default:
                        return "";

                }
            }
            set { }
        }
    }

    #endregion General

    #region LifeCycle

    [Serializable]
    public class LifeCycle
    {
        [SerializeField]
        private LangString version;
        [SerializeField]
        [LomTypeEnum("LOMv1.0", typeof(Lom1StatusEnum))]
        private Status status;
        [SerializeField]
        private LifeCycleContribute[] contribute;

        public LangString Version { get => version; set => version = value; }
        public Status Status { get => status; set => status = value; }
        public LifeCycleContribute[] Contribute { get => contribute; set => contribute = value; }
    }
    [Serializable]
    public class Status : LomType { }

    [Serializable]
    public enum Lom1StatusEnum
    {
        None, Draft, Final, Revised, Unavaliable
    }
    public class Lom1Status : Status
    {
        public enum Lom1StatusValues
        {
            None, Draft, Final, Revised, Unavaliable
        }
        public Lom1StatusValues EnumValue { get; set; } = Lom1StatusValues.None;

        public override string Source { get { return "LOMv1.0"; } set { } }

        public override string Value
        {
            get
            {
                switch (EnumValue)
                {
                    case Lom1StatusValues.Draft:
                        return "draft";
                    case Lom1StatusValues.Final:
                        return "final";
                    case Lom1StatusValues.Revised:
                        return "revised";
                    case Lom1StatusValues.Unavaliable:
                        return "unavailable";
                    default:
                        return "";

                }
            }
            set { }
        }
    }

    [Serializable]
    public class Contribute
    {
        [SerializeField]
        private string[] entity;
        [SerializeField]
        private LomDateTime date;

        public string[] Entity { get => entity; set => entity = value; }


        public LomDateTime GetDate()
        {
            return date;
        }

        public void SetDate(LomDateTime value)
        {
            date = value;
        }
    }

    [Serializable]
    public class LomDateTime
    {
        [SerializeField]
        private DateTime dateTime;
        [SerializeField]
        private LangString description;

        public DateTime DateTime { get => dateTime; set => dateTime = value; }
        public LangString Description { get => description; set => description = value; }
    }

    [Serializable]
    public class LifeCycleContribute : Contribute
    {
        [SerializeField]
        [LomTypeEnum("LOMv1.0", typeof(Lom1LifeCycleContributeRoleEnum))]
        private LifeCicleContributeRole role;

        public LifeCicleContributeRole Role { get => role; set => role = value; }

    }

    [Serializable]
    public class LifeCicleContributeRole : LomType { }

    [Serializable]
    public enum Lom1LifeCycleContributeRoleEnum
    {
        None, Author, Publisher, Unknown, Initiator, Terminator, Validator, Editor, GraphicalDesigner,
        TechnicalImplementer, ContentProvider, TechnicalValidator, EducationalValidator, ScriptWriter,
        InstructionalDesigner, SubjectMatterExpert
    }

    public class Lom1LifeCycleContributeRole : LifeCicleContributeRole
    {
        public enum Lom1LifeCycleContributeRoleValues
        {
            None, Author, Publisher, Unknown, Initiator, Terminator, Validator, Editor, GraphicalDesigner,
            TechnicalImplementer, ContentProvider, TechnicalValidator, EducationalValidator, ScriptWriter,
            InstructionalDesigner, SubjectMatterExpert
        }
        public Lom1LifeCycleContributeRoleValues EnumValue { get; set; } = Lom1LifeCycleContributeRoleValues.None;

        public override string Source { get { return "LOMv1.0"; } set { } }

        public override string Value
        {
            get
            {
                switch (EnumValue)
                {
                    case Lom1LifeCycleContributeRoleValues.Author:
                        return "author";
                    case Lom1LifeCycleContributeRoleValues.Publisher:
                        return "publisher";
                    case Lom1LifeCycleContributeRoleValues.Unknown:
                        return "unknown";
                    case Lom1LifeCycleContributeRoleValues.Initiator:
                        return "initiator";
                    case Lom1LifeCycleContributeRoleValues.Terminator:
                        return "terminator";
                    case Lom1LifeCycleContributeRoleValues.Validator:
                        return "validator";
                    case Lom1LifeCycleContributeRoleValues.Editor:
                        return "editor";
                    case Lom1LifeCycleContributeRoleValues.GraphicalDesigner:
                        return "graphical designer";
                    case Lom1LifeCycleContributeRoleValues.TechnicalImplementer:
                        return "technical implementer";
                    case Lom1LifeCycleContributeRoleValues.ContentProvider:
                        return "content provider";
                    case Lom1LifeCycleContributeRoleValues.TechnicalValidator:
                        return "technical validator";
                    case Lom1LifeCycleContributeRoleValues.EducationalValidator:
                        return "educational validator";
                    case Lom1LifeCycleContributeRoleValues.ScriptWriter:
                        return "script writer";
                    case Lom1LifeCycleContributeRoleValues.InstructionalDesigner:
                        return "instructional designer";
                    case Lom1LifeCycleContributeRoleValues.SubjectMatterExpert:
                        return "subject matter expert";
                    default:
                        return "";

                }
            }
            set { }
        }
    }

    #endregion LifeCycle

    #region MetaMetaData

    [Serializable]
    public class MetaMetaData
    {
        [SerializeField]
        private Identifier[] identifier;
        [SerializeField]
        private MetaDataContribute[] contribute;
        [SerializeField]
        private string[] metaDataSchema;
        [SerializeField]
        private string language;

        public Identifier[] Identifier { get => identifier; set => identifier = value; }

        public MetaDataContribute[] Contribute { get => contribute; set => contribute = value; }

        public string[] MetaDataSchema { get => metaDataSchema; set => metaDataSchema = value; }

        public string Language { get => language; set => language = value; }

    }

    [Serializable]
    public class MetaDataContribute : Contribute
    {
        [SerializeField]
        [LomTypeEnum("LOMv1.0", typeof(Lom1MetaDataContributeRoleEnum))]
        private MetaDataContributeRole contribute;

        public MetaDataContributeRole Contribute { get => contribute; set => contribute = value; }
    }

    [Serializable]
    public class MetaDataContributeRole : LomType { }

    [Serializable]
    public enum Lom1MetaDataContributeRoleEnum
    {
        None, Creator, Validator
    }

    public class Lom1MetaDataContributeRole : LifeCicleContributeRole
    {
        public enum Lom1MetaDataContributeRoleValues
        {
            None, Creator, Validator
        }
        public Lom1MetaDataContributeRoleValues EnumValue { get; set; } = Lom1MetaDataContributeRoleValues.None;

        public override string Source { get { return "LOMv1.0"; } set { } }

        public override string Value
        {
            get
            {
                switch (EnumValue)
                {
                    case Lom1MetaDataContributeRoleValues.Creator:
                        return "creator";
                    case Lom1MetaDataContributeRoleValues.Validator:
                        return "validator";
                    default:
                        return "";

                }
            }
            set { }
        }
    }

    #endregion MetaMetaData

    #region Technical

    [Serializable]
    public class Technical
    {
        [SerializeField]
        private string[] format;
        [SerializeField]
        private string size;
        [SerializeField]
        private string[] location;
        [SerializeField]
        private Requirement[] requirement;
        [SerializeField]
        private LangString installationRemarks;
        [SerializeField]
        private LangString otherPlatformRequirements;
        [SerializeField]
        private Duration duration;

        public string[] Format { get => format; set => format = value; }
        public string Size { get => size; set => size = value; }
        public string[] Location { get => location; set => location = value; }
        public Requirement[] Requirement { get => requirement; set => requirement = value; }
        public LangString InstallationRemarks { get => installationRemarks; set => installationRemarks = value; }
        public LangString OtherPlatformRequirements { get => otherPlatformRequirements; set => otherPlatformRequirements = value; }
        public Duration Duration { get => duration; set => duration = value; }
    }

    [Serializable]
    public class Requirement
    {
        [SerializeField]
        private OrComposite[] orComposite;

        public OrComposite[] OrComposite { get => orComposite; set => orComposite = value; }
    }
    [Serializable]
    public class OrComposite
    {
        [SerializeField]
        [LomTypeEnum("LOMv1.0", typeof(Lom1OrCompositeTypeEnum))]
        private OrCompositeType type;
        [SerializeField]
        [LomTypeEnum("LOMv1.0", typeof(Lom1OrCompositeNameEnum))]
        private OrCompositeName name;
        [SerializeField]
        private string minimumVersion;
        [SerializeField]
        private string maximumVersion;

        public OrCompositeType Type { get => type; set => type = value; }
        public OrCompositeName Name { get => name; set => name = value; }
        public string MinimumVersion { get => minimumVersion; set => minimumVersion = value; }
        public string MaximumVersion { get => maximumVersion; set => maximumVersion = value; }
    }
    [Serializable]
    public class OrCompositeType : LomType { }
    [Serializable]
    public enum Lom1OrCompositeTypeEnum
    {
        None, OperatingSystem, Browser
    }
    public class Lom1OrCompositeType : OrCompositeType
    {
        public enum Lom1OrCompositeTypeValues
        {
            None, OperatingSystem, Browser
        }
        public Lom1OrCompositeTypeValues EnumValue { get; set; } = Lom1OrCompositeTypeValues.None;

        public override string Source { get { return "LOMv1.0"; } set { } }

        public override string Value
        {
            get
            {
                switch (EnumValue)
                {
                    case Lom1OrCompositeTypeValues.OperatingSystem:
                        return "operating system";
                    case Lom1OrCompositeTypeValues.Browser:
                        return "browser";
                    default:
                        return "";

                }
            }
            set { }
        }
    }
    [Serializable]
    public class OrCompositeName : LomType { }
    [Serializable]
    public enum Lom1OrCompositeNameEnum
    {
        None, PcDos, MsWindows, MacOs, Unix, MultiOs, Any, NetscapeCommunicator, MsInternetExplorer, Opera, Amaya
    }
    public class Lom1OrCompositeName : OrCompositeType
    {
        public enum Lom1OrCompositeNameValues
        {
            None, PcDos, MsWindows, MacOs, Unix, MultiOs, Any, NetscapeCommunicator, MsInternetExplorer, Opera, Amaya
        }
        public Lom1OrCompositeNameValues EnumValue { get; set; } = Lom1OrCompositeNameValues.None;

        public override string Source { get { return "LOMv1.0"; } set { } }

        public override string Value
        {
            get
            {
                switch (EnumValue)
                {

                    case Lom1OrCompositeNameValues.PcDos:
                        return "pc-dos";
                    case Lom1OrCompositeNameValues.MsWindows:
                        return "ms-windows";
                    case Lom1OrCompositeNameValues.MacOs:
                        return "macos";
                    case Lom1OrCompositeNameValues.Unix:
                        return "unix";
                    case Lom1OrCompositeNameValues.MultiOs:
                        return "multi-os";
                    case Lom1OrCompositeNameValues.None:
                        return "none";
                    case Lom1OrCompositeNameValues.Any:
                        return "any";
                    case Lom1OrCompositeNameValues.NetscapeCommunicator:
                        return "netscape communicator";
                    case Lom1OrCompositeNameValues.MsInternetExplorer:
                        return "ms-internet explorer";
                    case Lom1OrCompositeNameValues.Opera:
                        return "opera";
                    case Lom1OrCompositeNameValues.Amaya:
                        return "amaya";
                    default:
                        return "";

                }
            }
            set { }
        }
    }
    [Serializable]
    public class Duration
    {
        [SerializeField]
        private string durationValue;
        [SerializeField]
        private LangString description;

        public string DurationValue { get => durationValue; set => durationValue = value; }
        public LangString Description { get => description; set => description = value; }
    }

    #endregion Technical

    #region Educational

    [Serializable]
    public class Educational
    {
        [SerializeField]
        [LomTypeEnum("LOMv1.0", typeof(Lom1InteractivityTypeEnum))]
        private InteractivityType interactivityType;
        [SerializeField]
        [LomTypeEnum("LOMv1.0", typeof(Lom1LearningResourceTypeEnum))]
        private LearningResourceType[] learningResourceType;
        [SerializeField]
        [LomTypeEnum("LOMv1.0", typeof(Lom1InteractivityLevelEnum))]
        private InteractivityLevel interactivityLevel;
        [SerializeField]
        [LomTypeEnum("LOMv1.0", typeof(Lom1SemanticDensityEnum))]
        private SemanticDensity semanticDensity;
        [SerializeField]
        [LomTypeEnum("LOMv1.0", typeof(Lom1IntendedEndUserRoleEnum))]
        private IntendedEndUserRole[] intendedEndUserRole;
        [SerializeField]
        [LomTypeEnum("LOMv1.0", typeof(Lom1EducationalContextEnum))]
        private EducationalContext[] context;
        [SerializeField]
        private LangString[] typicalAgeRange;
        [SerializeField]
        [LomTypeEnum("LOMv1.0", typeof(Lom1DifficultyEnum))]
        private Difficulty difficulty;
        [SerializeField]
        private Duration typicalLearningTime;
        [SerializeField]
        private LangString description;
        [SerializeField]
        private string[] language;

        public InteractivityType InteractivityType { get => interactivityType; set => interactivityType = value; }
        public LearningResourceType[] LearningResourceType { get => learningResourceType; set => learningResourceType = value; }
        public InteractivityLevel InteractivityLevel { get => interactivityLevel; set => interactivityLevel = value; }
        public SemanticDensity SemanticDensity { get => semanticDensity; set => semanticDensity = value; }
        public IntendedEndUserRole[] IntendedEndUserRole { get => intendedEndUserRole; set => intendedEndUserRole = value; }
        public EducationalContext[] Context { get => context; set => context = value; }
        public LangString[] TypicalAgeRange { get => typicalAgeRange; set => typicalAgeRange = value; }
        public Difficulty Difficulty { get => difficulty; set => difficulty = value; }
        public Duration TypicalLearningTime { get => typicalLearningTime; set => typicalLearningTime = value; }
        public LangString Description { get => description; set => description = value; }
        public string[] Language { get => language; set => language = value; }

    }

    [Serializable]
    public class InteractivityType : LomType { }
    [Serializable]
    public enum Lom1InteractivityTypeEnum
    {
        None, Active, Expositive, Mixed
    }
    public class Lom1InteractivityType : InteractivityType
    {
        public enum Values
        {
            None, Active, Expositive, Mixed
        }
        public Values EnumValue { get; set; } = Values.None;

        public override string Source { get { return "LOMv1.0"; } set { } }

        public override string Value
        {
            get
            {
                switch (EnumValue)
                {
                    case Values.Active:
                        return "active";
                    case Values.Expositive:
                        return "expositive";
                    case Values.Mixed:
                        return "mixed";
                    default:
                        return "";

                }
            }
            set { }
        }
    }
    [Serializable]
    public class LearningResourceType : LomType { }
    [Serializable]
    public enum Lom1LearningResourceTypeEnum
    {
        None, Exercise, Simulation, Questionnaire, Diagram, Figure, Graph, Index, Slide, Table, NarrativeText, Exam, Experiment, ProblemStatement, SelfAssessment, Lecture
    }
    public class Lom1LearningResourceType : LearningResourceType
    {
        public enum Values
        {
            None, Exercise, Simulation, Questionnaire, Diagram, Figure, Graph, Index, Slide, Table, NarrativeText, Exam, Experiment, ProblemStatement, SelfAssessment, Lecture
        }
        public Values EnumValue { get; set; } = Values.None;

        public override string Source { get { return "LOMv1.0"; } set { } }

        public override string Value
        {
            get
            {
                switch (EnumValue)
                {
                    case Values.Exercise:
                        return "exercise";
                    case Values.Simulation:
                        return "simulation";
                    case Values.Questionnaire:
                        return "questionnaire";
                    case Values.Diagram:
                        return "diagram";
                    case Values.Figure:
                        return "figure";
                    case Values.Graph:
                        return "graph";
                    case Values.Index:
                        return "index";
                    case Values.Slide:
                        return "slide";
                    case Values.Table:
                        return "table";
                    case Values.NarrativeText:
                        return "narrative text";
                    case Values.Exam:
                        return "exam";
                    case Values.Experiment:
                        return "experiment";
                    case Values.ProblemStatement:
                        return "problem statement";
                    case Values.SelfAssessment:
                        return "self assessment";
                    case Values.Lecture:
                        return "lecture";
                    default:
                        return "";

                }
            }
            set { }
        }
    }
    [Serializable]
    public class InteractivityLevel : LomType { }
    [Serializable]
    public enum Lom1InteractivityLevelEnum
    {
        None, VeryLow, Low, Medium, High, VeryHigh
    }
    public class Lom1InteractivityLevel : InteractivityLevel
    {
        public enum Values
        {
            None, VeryLow, Low, Medium, High, VeryHigh
        }
        public Values EnumValue { get; set; } = Values.None;

        public override string Source { get { return "LOMv1.0"; } set { } }

        public override string Value
        {
            get
            {
                switch (EnumValue)
                {
                    case Values.VeryLow:
                        return "very low";
                    case Values.Low:
                        return "low";
                    case Values.Medium:
                        return "medium";
                    case Values.High:
                        return "high";
                    case Values.VeryHigh:
                        return "very high";
                    default:
                        return "";

                }
            }
            set { }
        }
    }
    [Serializable]
    public class SemanticDensity : LomType { }
    [Serializable]
    public enum Lom1SemanticDensityEnum
    {
        None, VeryLow, Low, Medium, High, VeryHigh
    }
    public class Lom1SemanticDensity : SemanticDensity
    {
        public enum Values
        {
            None, VeryLow, Low, Medium, High, VeryHigh
        }
        public Values EnumValue { get; set; } = Values.None;

        public override string Source { get { return "LOMv1.0"; } set { } }

        public override string Value
        {
            get
            {
                switch (EnumValue)
                {
                    case Values.VeryLow:
                        return "very low";
                    case Values.Low:
                        return "low";
                    case Values.Medium:
                        return "medium";
                    case Values.High:
                        return "high";
                    case Values.VeryHigh:
                        return "very high";
                    default:
                        return "";

                }
            }
            set { }
        }
    }
    [Serializable]
    public class IntendedEndUserRole : LomType { }
    [Serializable]
    public enum Lom1IntendedEndUserRoleEnum
    {
        None, Teacher, Author, Learner, Manager
    }
    public class Lom1IntendedEndUserRole : IntendedEndUserRole
    {
        public enum Values
        {
            None, Teacher, Author, Learner, Manager
        }
        public Values EnumValue { get; set; } = Values.None;

        public override string Source { get { return "LOMv1.0"; } set { } }

        public override string Value
        {
            get
            {
                switch (EnumValue)
                {
                    case Values.Teacher:
                        return "teacher";
                    case Values.Author:
                        return "author";
                    case Values.Learner:
                        return "learner";
                    case Values.Manager:
                        return "manager";
                    default:
                        return "";

                }
            }
            set { }
        }
    }
    [Serializable]
    public class EducationalContext : LomType { }
    [Serializable]
    public enum Lom1EducationalContextEnum
    {
        None, School, HigherEducation, Training, Other
    }
    public class Lom1EducationalContext : EducationalContext
    {
        public enum Values
        {
            None, School, HigherEducation, Training, Other
        }
        public Values EnumValue { get; set; } = Values.None;

        public override string Source { get { return "LOMv1.0"; } set { } }

        public override string Value
        {
            get
            {
                switch (EnumValue)
                {
                    case Values.School:
                        return "school";
                    case Values.HigherEducation:
                        return "higher education";
                    case Values.Training:
                        return "training";
                    case Values.Other:
                        return "other";
                    default:
                        return "";

                }
            }
            set { }
        }
    }
    [Serializable]
    public class Difficulty : LomType { }
    [Serializable]
    public enum Lom1DifficultyEnum
    {
        None, VeryEasy, Easy, Medium, Difficult, VeryDifficult
    }
    public class Lom1Difficulty : Difficulty
    {
        public enum Values
        {
            None, VeryEasy, Easy, Medium, Difficult, VeryDifficult
        }
        public Values EnumValue { get; set; } = Values.None;

        public override string Source { get { return "LOMv1.0"; } set { } }

        public override string Value
        {
            get
            {
                switch (EnumValue)
                {
                    case Values.VeryEasy:
                        return "very easy";
                    case Values.Easy:
                        return "easy";
                    case Values.Medium:
                        return "medium";
                    case Values.Difficult:
                        return "difficult";
                    case Values.VeryDifficult:
                        return "very difficult";
                    default:
                        return "";

                }
            }
            set { }
        }
    }

    #endregion Educational

    #region Rights

    [Serializable]
    public class Rights
    {
        [SerializeField]
        [LomTypeEnum("LOMv1.0", typeof(Lom1CostEnum))]
        private Cost cost;
        [SerializeField]
        [LomTypeEnum("LOMv1.0", typeof(Lom1CopyrightAndOtherRestrictionsEnum))]
        private CopyrightAndOtherRestrictions copyrightAndOtherRestrictions;
        [SerializeField]
        private LangString description;

        public Cost Cost { get => cost; set => cost = value; }
        public CopyrightAndOtherRestrictions CopyrightAndOtherRestrictions { get => copyrightAndOtherRestrictions; set => copyrightAndOtherRestrictions = value; }
        public LangString Description { get => description; set => description = value; }
    }

    [Serializable]
    public class Cost : LomType { }
    [Serializable]
    public enum Lom1CostEnum
    {
        None, Yes, No
    }
    public class Lom1Cost : Cost
    {
        public enum Values
        {
            None, Yes, No
        }
        public Values EnumValue { get; set; } = Values.None;

        public override string Source { get { return "LOMv1.0"; } set { } }

        public override string Value
        {
            get
            {
                switch (EnumValue)
                {
                    case Values.Yes:
                        return "yes";
                    case Values.No:
                        return "no";
                    default:
                        return "";

                }
            }
            set { }
        }
    }
    [Serializable]
    public class CopyrightAndOtherRestrictions : LomType { }
    [Serializable]
    public enum Lom1CopyrightAndOtherRestrictionsEnum
    {
        None, Yes, No
    }
    public class Lom1CopyrightAndOtherRestrictions : CopyrightAndOtherRestrictions
    {
        public enum Values
        {
            None, Yes, No
        }
        public Values EnumValue { get; set; } = Values.None;

        public override string Source { get { return "LOMv1.0"; } set { } }

        public override string Value
        {
            get
            {
                switch (EnumValue)
                {
                    case Values.Yes:
                        return "yes";
                    case Values.No:
                        return "no";
                    default:
                        return "";

                }
            }
            set { }
        }
    }

    #endregion Rights

    #region Relation

    [Serializable]
    public class Relation
    {
        [SerializeField]
        [LomTypeEnum("LOMv1.0", typeof(Lom1KindEnum))]
        private Kind kind;
        [SerializeField]
        private Resource resource;

        public Kind Kind { get => kind; set => kind = value; }
        public Resource Resource { get => resource; set => resource = value; }
    }
    [Serializable]
    public class Kind : LomType { }
    [Serializable]
    public enum Lom1KindEnum
    {
        None, IsPartOf, HasPart, IsVersionOf, HasVersion, IsFormatOf, HasFormat, References, IsReferencedBy,
        IsBasedOn, IsBasisFor, Requires, IsRequiredBy
    }
    public class Lom1Kind : Kind
    {
        public enum Values
        {
            None, IsPartOf, HasPart, IsVersionOf, HasVersion, IsFormatOf, HasFormat, References, IsReferencedBy,
            IsBasedOn, IsBasisFor, Requires, IsRequiredBy
        }
        public Values EnumValue { get; set; } = Values.None;

        public override string Source { get { return "LOMv1.0"; } set { } }

        public override string Value
        {
            get
            {
                switch (EnumValue)
                {
                    case Values.IsPartOf:         return "ispartof";
                    case Values.HasPart:          return "haspart";
                    case Values.IsVersionOf:      return "isversionof";
                    case Values.HasVersion:       return "hasversion";
                    case Values.IsFormatOf:       return "isformatof";
                    case Values.HasFormat:        return "hasformat";
                    case Values.References:       return "references";
                    case Values.IsReferencedBy:   return "isreferencedby";
                    case Values.IsBasedOn:        return "isbasedon";
                    case Values.IsBasisFor:       return "isbasisfor";
                    case Values.Requires:         return "requires";
                    case Values.IsRequiredBy:     return "isrequiredby";
                    default:
                        return "";
                }
            }
            set { }
        }
    }
    [Serializable]
    public class Resource
    {
        [SerializeField]
        private Identifier[] identifier;
        [SerializeField]
        private LangString[] description;

        public Identifier[] Identifier { get => identifier; set => identifier = value; }
        public LangString[] Description { get => description; set => description = value; }
    }

    #endregion Relation

    #region Annotation

    [Serializable]
    public class Annotation
    {
        [SerializeField]
        private string entity;
        [SerializeField]
        private LomDateTime date;
        [SerializeField]
        private LangString description;

        public string Entity { get => entity; set => entity = value; }
        public LomDateTime Date { get => date; set => date = value; }
        public LangString Description { get => description; set => description = value; }
    }

    #endregion Annotation

    #region Classification

    [Serializable]
    public class Classification
    {
        [SerializeField]
        [LomTypeEnum("LOMv1.0", typeof(Lom1PurposeEnum))]
        private Purpose purpose;
        [SerializeField]
        private TaxonPath[] taxonPath;

        public Purpose Purpose { get => purpose; set => purpose = value; }
        public TaxonPath[] TaxonPath { get => taxonPath; set => taxonPath = value; }
        public LangString Description { get; set; }
        public LangString[] Keyword { get; set; }
    }

    [Serializable]
    public class Purpose : LomType { }
    [Serializable]
    public enum Lom1PurposeEnum
    {
        None,
        Discipline,
        Idea,
        Prerequisite,
        EducationalObjective,
        AccessibilityRestrictions,
        EducationalLevel,
        SkillLevel,
        SecurityLevel,
        Competency
    }
    public class Lom1Purpose : Purpose
    {
        public enum Values
        {
            None,
            Discipline,
            Idea,
            Prerequisite,
            EducationalObjective,
            AccessibilityRestrictions,
            EducationalLevel,
            SkillLevel,
            SecurityLevel,
            Competency
        }
        public Values EnumValue { get; set; } = Values.None;

        public override string Source { get { return "LOMv1.0"; } set { } }

        public override string Value
        {
            get
            {
                switch (EnumValue)
                {
                    case Values.Discipline:
                        return "discipline";
                    case Values.Idea:
                        return "idea";
                    case Values.Prerequisite:
                        return "prerequisite";
                    case Values.EducationalObjective:
                        return "educational objective";
                    case Values.AccessibilityRestrictions:
                        return "accessibility restrictions";
                    case Values.EducationalLevel:
                        return "educational level";
                    case Values.SkillLevel:
                        return "skill level";
                    case Values.SecurityLevel:
                        return "security level";
                    case Values.Competency:
                        return "competency";
                    default:
                        return "";

                }
            }
            set { }
        }
    }

    [Serializable]
    public class TaxonPath
    {
        [SerializeField]
        private LangString source;
        [SerializeField]
        private Taxon[] taxon;

        public LangString Source { get => source; set => source = value; }
        public Taxon[] Taxon { get => taxon; set => taxon = value; }
    }

    [Serializable]
    public class Taxon
    {
        [SerializeField]
        private string id;
        [SerializeField]
        private LangString entry;

        public string Id { get => id; set => id = value; }
        public LangString Entry { get => entry; set => entry = value; }
    }

    #endregion Classification
}
