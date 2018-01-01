﻿namespace ATN.Crawler.MAS
{
    using System.Runtime.Serialization;
    using System;


    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "ResultCollection", Namespace = "http://research.microsoft.com")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(ATN.Crawler.MAS.AuthorResponse))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(ATN.Crawler.MAS.ConferenceResponse))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(ATN.Crawler.MAS.JournalResponse))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(ATN.Crawler.MAS.KeywordResponse))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(ATN.Crawler.MAS.OrganizationResponse))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(ATN.Crawler.MAS.PublicationResponse))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(ATN.Crawler.MAS.DomainResponse))]
    public partial class ResultCollection : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint EndIdxField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint StartIdxField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint TotalItemField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint EndIdx
        {
            get
            {
                return this.EndIdxField;
            }
            set
            {
                if ((this.EndIdxField.Equals(value) != true))
                {
                    this.EndIdxField = value;
                    this.RaisePropertyChanged("EndIdx");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint StartIdx
        {
            get
            {
                return this.StartIdxField;
            }
            set
            {
                if ((this.StartIdxField.Equals(value) != true))
                {
                    this.StartIdxField = value;
                    this.RaisePropertyChanged("StartIdx");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint TotalItem
        {
            get
            {
                return this.TotalItemField;
            }
            set
            {
                if ((this.TotalItemField.Equals(value) != true))
                {
                    this.TotalItemField = value;
                    this.RaisePropertyChanged("TotalItem");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "AuthorResponse", Namespace = "http://research.microsoft.com")]
    [System.SerializableAttribute()]
    public partial class AuthorResponse : ATN.Crawler.MAS.ResultCollection
    {

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.Author[] ResultField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.Author[] Result
        {
            get
            {
                return this.ResultField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ResultField, value) != true))
                {
                    this.ResultField = value;
                    this.RaisePropertyChanged("Result");
                }
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "ConferenceResponse", Namespace = "http://research.microsoft.com")]
    [System.SerializableAttribute()]
    public partial class ConferenceResponse : ATN.Crawler.MAS.ResultCollection
    {

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.Conference[] ResultField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.Conference[] Result
        {
            get
            {
                return this.ResultField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ResultField, value) != true))
                {
                    this.ResultField = value;
                    this.RaisePropertyChanged("Result");
                }
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "JournalResponse", Namespace = "http://research.microsoft.com")]
    [System.SerializableAttribute()]
    public partial class JournalResponse : ATN.Crawler.MAS.ResultCollection
    {

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.Journal[] ResultField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.Journal[] Result
        {
            get
            {
                return this.ResultField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ResultField, value) != true))
                {
                    this.ResultField = value;
                    this.RaisePropertyChanged("Result");
                }
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "KeywordResponse", Namespace = "http://research.microsoft.com")]
    [System.SerializableAttribute()]
    public partial class KeywordResponse : ATN.Crawler.MAS.ResultCollection
    {

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.Keyword[] ResultField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.Keyword[] Result
        {
            get
            {
                return this.ResultField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ResultField, value) != true))
                {
                    this.ResultField = value;
                    this.RaisePropertyChanged("Result");
                }
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "OrganizationResponse", Namespace = "http://research.microsoft.com")]
    [System.SerializableAttribute()]
    public partial class OrganizationResponse : ATN.Crawler.MAS.ResultCollection
    {

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.Organization[] ResultField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.Organization[] Result
        {
            get
            {
                return this.ResultField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ResultField, value) != true))
                {
                    this.ResultField = value;
                    this.RaisePropertyChanged("Result");
                }
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "PublicationResponse", Namespace = "http://research.microsoft.com")]
    [System.SerializableAttribute()]
    public partial class PublicationResponse : ATN.Crawler.MAS.ResultCollection
    {

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.Publication[] ResultField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.Publication[] Result
        {
            get
            {
                return this.ResultField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ResultField, value) != true))
                {
                    this.ResultField = value;
                    this.RaisePropertyChanged("Result");
                }
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "DomainResponse", Namespace = "http://research.microsoft.com")]
    [System.SerializableAttribute()]
    public partial class DomainResponse : ATN.Crawler.MAS.ResultCollection
    {

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.Domain[] ResultField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.Domain[] Result
        {
            get
            {
                return this.ResultField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ResultField, value) != true))
                {
                    this.ResultField = value;
                    this.RaisePropertyChanged("Result");
                }
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "Domain", Namespace = "http://research.microsoft.com")]
    [System.SerializableAttribute()]
    public partial class Domain : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint CitationCountField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint DomainIDField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint PublicationCountField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint SubDomainIDField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint CitationCount
        {
            get
            {
                return this.CitationCountField;
            }
            set
            {
                if ((this.CitationCountField.Equals(value) != true))
                {
                    this.CitationCountField = value;
                    this.RaisePropertyChanged("CitationCount");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint DomainID
        {
            get
            {
                return this.DomainIDField;
            }
            set
            {
                if ((this.DomainIDField.Equals(value) != true))
                {
                    this.DomainIDField = value;
                    this.RaisePropertyChanged("DomainID");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name
        {
            get
            {
                return this.NameField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NameField, value) != true))
                {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint PublicationCount
        {
            get
            {
                return this.PublicationCountField;
            }
            set
            {
                if ((this.PublicationCountField.Equals(value) != true))
                {
                    this.PublicationCountField = value;
                    this.RaisePropertyChanged("PublicationCount");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint SubDomainID
        {
            get
            {
                return this.SubDomainIDField;
            }
            set
            {
                if ((this.SubDomainIDField.Equals(value) != true))
                {
                    this.SubDomainIDField = value;
                    this.RaisePropertyChanged("SubDomainID");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "Author", Namespace = "http://research.microsoft.com")]
    [System.SerializableAttribute()]
    public partial class Author : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.Organization AffiliationField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint CitationCountField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DisplayPhotoURLField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FirstNameField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint GIndexField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint HIndexField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string HomepageURLField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint IDField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LastNameField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MiddleNameField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NativeNameField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint PublicationCountField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.Domain[] ResearchInterestDomainField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.Organization Affiliation
        {
            get
            {
                return this.AffiliationField;
            }
            set
            {
                if ((object.ReferenceEquals(this.AffiliationField, value) != true))
                {
                    this.AffiliationField = value;
                    this.RaisePropertyChanged("Affiliation");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint CitationCount
        {
            get
            {
                return this.CitationCountField;
            }
            set
            {
                if ((this.CitationCountField.Equals(value) != true))
                {
                    this.CitationCountField = value;
                    this.RaisePropertyChanged("CitationCount");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DisplayPhotoURL
        {
            get
            {
                return this.DisplayPhotoURLField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DisplayPhotoURLField, value) != true))
                {
                    this.DisplayPhotoURLField = value;
                    this.RaisePropertyChanged("DisplayPhotoURL");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FirstName
        {
            get
            {
                return this.FirstNameField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FirstNameField, value) != true))
                {
                    this.FirstNameField = value;
                    this.RaisePropertyChanged("FirstName");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint GIndex
        {
            get
            {
                return this.GIndexField;
            }
            set
            {
                if ((this.GIndexField.Equals(value) != true))
                {
                    this.GIndexField = value;
                    this.RaisePropertyChanged("GIndex");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint HIndex
        {
            get
            {
                return this.HIndexField;
            }
            set
            {
                if ((this.HIndexField.Equals(value) != true))
                {
                    this.HIndexField = value;
                    this.RaisePropertyChanged("HIndex");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string HomepageURL
        {
            get
            {
                return this.HomepageURLField;
            }
            set
            {
                if ((object.ReferenceEquals(this.HomepageURLField, value) != true))
                {
                    this.HomepageURLField = value;
                    this.RaisePropertyChanged("HomepageURL");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint ID
        {
            get
            {
                return this.IDField;
            }
            set
            {
                if ((this.IDField.Equals(value) != true))
                {
                    this.IDField = value;
                    this.RaisePropertyChanged("ID");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LastName
        {
            get
            {
                return this.LastNameField;
            }
            set
            {
                if ((object.ReferenceEquals(this.LastNameField, value) != true))
                {
                    this.LastNameField = value;
                    this.RaisePropertyChanged("LastName");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string MiddleName
        {
            get
            {
                return this.MiddleNameField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MiddleNameField, value) != true))
                {
                    this.MiddleNameField = value;
                    this.RaisePropertyChanged("MiddleName");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NativeName
        {
            get
            {
                return this.NativeNameField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NativeNameField, value) != true))
                {
                    this.NativeNameField = value;
                    this.RaisePropertyChanged("NativeName");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint PublicationCount
        {
            get
            {
                return this.PublicationCountField;
            }
            set
            {
                if ((this.PublicationCountField.Equals(value) != true))
                {
                    this.PublicationCountField = value;
                    this.RaisePropertyChanged("PublicationCount");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.Domain[] ResearchInterestDomain
        {
            get
            {
                return this.ResearchInterestDomainField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ResearchInterestDomainField, value) != true))
                {
                    this.ResearchInterestDomainField = value;
                    this.RaisePropertyChanged("ResearchInterestDomain");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "Organization", Namespace = "http://research.microsoft.com")]
    [System.SerializableAttribute()]
    public partial class Organization : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint AuthorCountField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint CitationCountField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string HomepageURLField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint IDField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint PublicationCountField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.Domain[] ResearchInterestDomainField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint AuthorCount
        {
            get
            {
                return this.AuthorCountField;
            }
            set
            {
                if ((this.AuthorCountField.Equals(value) != true))
                {
                    this.AuthorCountField = value;
                    this.RaisePropertyChanged("AuthorCount");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint CitationCount
        {
            get
            {
                return this.CitationCountField;
            }
            set
            {
                if ((this.CitationCountField.Equals(value) != true))
                {
                    this.CitationCountField = value;
                    this.RaisePropertyChanged("CitationCount");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string HomepageURL
        {
            get
            {
                return this.HomepageURLField;
            }
            set
            {
                if ((object.ReferenceEquals(this.HomepageURLField, value) != true))
                {
                    this.HomepageURLField = value;
                    this.RaisePropertyChanged("HomepageURL");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint ID
        {
            get
            {
                return this.IDField;
            }
            set
            {
                if ((this.IDField.Equals(value) != true))
                {
                    this.IDField = value;
                    this.RaisePropertyChanged("ID");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name
        {
            get
            {
                return this.NameField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NameField, value) != true))
                {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint PublicationCount
        {
            get
            {
                return this.PublicationCountField;
            }
            set
            {
                if ((this.PublicationCountField.Equals(value) != true))
                {
                    this.PublicationCountField = value;
                    this.RaisePropertyChanged("PublicationCount");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.Domain[] ResearchInterestDomain
        {
            get
            {
                return this.ResearchInterestDomainField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ResearchInterestDomainField, value) != true))
                {
                    this.ResearchInterestDomainField = value;
                    this.RaisePropertyChanged("ResearchInterestDomain");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "Conference", Namespace = "http://research.microsoft.com")]
    [System.SerializableAttribute()]
    public partial class Conference : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.CFPInfo CFPField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint CitationCountField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ushort EndYearField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FullNameField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string HomepageURLField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint IDField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint PublicationCountField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.Domain[] ResearchInterestDomainField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ShortNameField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ushort StartYearField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.CFPInfo CFP
        {
            get
            {
                return this.CFPField;
            }
            set
            {
                if ((object.ReferenceEquals(this.CFPField, value) != true))
                {
                    this.CFPField = value;
                    this.RaisePropertyChanged("CFP");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint CitationCount
        {
            get
            {
                return this.CitationCountField;
            }
            set
            {
                if ((this.CitationCountField.Equals(value) != true))
                {
                    this.CitationCountField = value;
                    this.RaisePropertyChanged("CitationCount");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ushort EndYear
        {
            get
            {
                return this.EndYearField;
            }
            set
            {
                if ((this.EndYearField.Equals(value) != true))
                {
                    this.EndYearField = value;
                    this.RaisePropertyChanged("EndYear");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FullName
        {
            get
            {
                return this.FullNameField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FullNameField, value) != true))
                {
                    this.FullNameField = value;
                    this.RaisePropertyChanged("FullName");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string HomepageURL
        {
            get
            {
                return this.HomepageURLField;
            }
            set
            {
                if ((object.ReferenceEquals(this.HomepageURLField, value) != true))
                {
                    this.HomepageURLField = value;
                    this.RaisePropertyChanged("HomepageURL");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint ID
        {
            get
            {
                return this.IDField;
            }
            set
            {
                if ((this.IDField.Equals(value) != true))
                {
                    this.IDField = value;
                    this.RaisePropertyChanged("ID");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint PublicationCount
        {
            get
            {
                return this.PublicationCountField;
            }
            set
            {
                if ((this.PublicationCountField.Equals(value) != true))
                {
                    this.PublicationCountField = value;
                    this.RaisePropertyChanged("PublicationCount");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.Domain[] ResearchInterestDomain
        {
            get
            {
                return this.ResearchInterestDomainField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ResearchInterestDomainField, value) != true))
                {
                    this.ResearchInterestDomainField = value;
                    this.RaisePropertyChanged("ResearchInterestDomain");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ShortName
        {
            get
            {
                return this.ShortNameField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ShortNameField, value) != true))
                {
                    this.ShortNameField = value;
                    this.RaisePropertyChanged("ShortName");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ushort StartYear
        {
            get
            {
                return this.StartYearField;
            }
            set
            {
                if ((this.StartYearField.Equals(value) != true))
                {
                    this.StartYearField = value;
                    this.RaisePropertyChanged("StartYear");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "CFPInfo", Namespace = "http://research.microsoft.com")]
    [System.SerializableAttribute()]
    public partial class CFPInfo : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime AbstractSubmissionDateField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string AgendaUrlField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CityField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime ConferenceEndDateField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime ConferenceStartDateField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CountryField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime FinalVersionDateField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string HomepageURLField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime PaperSubmissionDateField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime ResultNotificationDateField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime AbstractSubmissionDate
        {
            get
            {
                return this.AbstractSubmissionDateField;
            }
            set
            {
                if ((this.AbstractSubmissionDateField.Equals(value) != true))
                {
                    this.AbstractSubmissionDateField = value;
                    this.RaisePropertyChanged("AbstractSubmissionDate");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string AgendaUrl
        {
            get
            {
                return this.AgendaUrlField;
            }
            set
            {
                if ((object.ReferenceEquals(this.AgendaUrlField, value) != true))
                {
                    this.AgendaUrlField = value;
                    this.RaisePropertyChanged("AgendaUrl");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string City
        {
            get
            {
                return this.CityField;
            }
            set
            {
                if ((object.ReferenceEquals(this.CityField, value) != true))
                {
                    this.CityField = value;
                    this.RaisePropertyChanged("City");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime ConferenceEndDate
        {
            get
            {
                return this.ConferenceEndDateField;
            }
            set
            {
                if ((this.ConferenceEndDateField.Equals(value) != true))
                {
                    this.ConferenceEndDateField = value;
                    this.RaisePropertyChanged("ConferenceEndDate");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime ConferenceStartDate
        {
            get
            {
                return this.ConferenceStartDateField;
            }
            set
            {
                if ((this.ConferenceStartDateField.Equals(value) != true))
                {
                    this.ConferenceStartDateField = value;
                    this.RaisePropertyChanged("ConferenceStartDate");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Country
        {
            get
            {
                return this.CountryField;
            }
            set
            {
                if ((object.ReferenceEquals(this.CountryField, value) != true))
                {
                    this.CountryField = value;
                    this.RaisePropertyChanged("Country");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime FinalVersionDate
        {
            get
            {
                return this.FinalVersionDateField;
            }
            set
            {
                if ((this.FinalVersionDateField.Equals(value) != true))
                {
                    this.FinalVersionDateField = value;
                    this.RaisePropertyChanged("FinalVersionDate");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string HomepageURL
        {
            get
            {
                return this.HomepageURLField;
            }
            set
            {
                if ((object.ReferenceEquals(this.HomepageURLField, value) != true))
                {
                    this.HomepageURLField = value;
                    this.RaisePropertyChanged("HomepageURL");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime PaperSubmissionDate
        {
            get
            {
                return this.PaperSubmissionDateField;
            }
            set
            {
                if ((this.PaperSubmissionDateField.Equals(value) != true))
                {
                    this.PaperSubmissionDateField = value;
                    this.RaisePropertyChanged("PaperSubmissionDate");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime ResultNotificationDate
        {
            get
            {
                return this.ResultNotificationDateField;
            }
            set
            {
                if ((this.ResultNotificationDateField.Equals(value) != true))
                {
                    this.ResultNotificationDateField = value;
                    this.RaisePropertyChanged("ResultNotificationDate");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "Journal", Namespace = "http://research.microsoft.com")]
    [System.SerializableAttribute()]
    public partial class Journal : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint CitationCountField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ushort EndYearField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FullNameField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string HomepageURLField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint IDField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ISSNField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint PublicationCountField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.Domain[] ResearchInterestDomainField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ShortNameField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ushort StartYearField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint CitationCount
        {
            get
            {
                return this.CitationCountField;
            }
            set
            {
                if ((this.CitationCountField.Equals(value) != true))
                {
                    this.CitationCountField = value;
                    this.RaisePropertyChanged("CitationCount");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ushort EndYear
        {
            get
            {
                return this.EndYearField;
            }
            set
            {
                if ((this.EndYearField.Equals(value) != true))
                {
                    this.EndYearField = value;
                    this.RaisePropertyChanged("EndYear");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FullName
        {
            get
            {
                return this.FullNameField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FullNameField, value) != true))
                {
                    this.FullNameField = value;
                    this.RaisePropertyChanged("FullName");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string HomepageURL
        {
            get
            {
                return this.HomepageURLField;
            }
            set
            {
                if ((object.ReferenceEquals(this.HomepageURLField, value) != true))
                {
                    this.HomepageURLField = value;
                    this.RaisePropertyChanged("HomepageURL");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint ID
        {
            get
            {
                return this.IDField;
            }
            set
            {
                if ((this.IDField.Equals(value) != true))
                {
                    this.IDField = value;
                    this.RaisePropertyChanged("ID");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ISSN
        {
            get
            {
                return this.ISSNField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ISSNField, value) != true))
                {
                    this.ISSNField = value;
                    this.RaisePropertyChanged("ISSN");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint PublicationCount
        {
            get
            {
                return this.PublicationCountField;
            }
            set
            {
                if ((this.PublicationCountField.Equals(value) != true))
                {
                    this.PublicationCountField = value;
                    this.RaisePropertyChanged("PublicationCount");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.Domain[] ResearchInterestDomain
        {
            get
            {
                return this.ResearchInterestDomainField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ResearchInterestDomainField, value) != true))
                {
                    this.ResearchInterestDomainField = value;
                    this.RaisePropertyChanged("ResearchInterestDomain");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ShortName
        {
            get
            {
                return this.ShortNameField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ShortNameField, value) != true))
                {
                    this.ShortNameField = value;
                    this.RaisePropertyChanged("ShortName");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ushort StartYear
        {
            get
            {
                return this.StartYearField;
            }
            set
            {
                if ((this.StartYearField.Equals(value) != true))
                {
                    this.StartYearField = value;
                    this.RaisePropertyChanged("StartYear");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "Keyword", Namespace = "http://research.microsoft.com")]
    [System.SerializableAttribute()]
    public partial class Keyword : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint CitationCountField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint IDField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint PublicationCountField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint CitationCount
        {
            get
            {
                return this.CitationCountField;
            }
            set
            {
                if ((this.CitationCountField.Equals(value) != true))
                {
                    this.CitationCountField = value;
                    this.RaisePropertyChanged("CitationCount");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint ID
        {
            get
            {
                return this.IDField;
            }
            set
            {
                if ((this.IDField.Equals(value) != true))
                {
                    this.IDField = value;
                    this.RaisePropertyChanged("ID");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name
        {
            get
            {
                return this.NameField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NameField, value) != true))
                {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint PublicationCount
        {
            get
            {
                return this.PublicationCountField;
            }
            set
            {
                if ((this.PublicationCountField.Equals(value) != true))
                {
                    this.PublicationCountField = value;
                    this.RaisePropertyChanged("PublicationCount");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "Publication", Namespace = "http://research.microsoft.com")]
    [System.SerializableAttribute()]
    public partial class Publication : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string AbstractField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.Author[] AuthorField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string[] CitationContextField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint CitationCountField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.Conference ConferenceField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DOIField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string[] FullVersionURLField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint IDField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.Journal JournalField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.Keyword[] KeywordField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint ReferenceCountField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TitleField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.PublicationType TypeField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ushort YearField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Abstract
        {
            get
            {
                return this.AbstractField;
            }
            set
            {
                if ((object.ReferenceEquals(this.AbstractField, value) != true))
                {
                    this.AbstractField = value;
                    this.RaisePropertyChanged("Abstract");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.Author[] Author
        {
            get
            {
                return this.AuthorField;
            }
            set
            {
                if ((object.ReferenceEquals(this.AuthorField, value) != true))
                {
                    this.AuthorField = value;
                    this.RaisePropertyChanged("Author");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string[] CitationContext
        {
            get
            {
                return this.CitationContextField;
            }
            set
            {
                if ((object.ReferenceEquals(this.CitationContextField, value) != true))
                {
                    this.CitationContextField = value;
                    this.RaisePropertyChanged("CitationContext");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint CitationCount
        {
            get
            {
                return this.CitationCountField;
            }
            set
            {
                if ((this.CitationCountField.Equals(value) != true))
                {
                    this.CitationCountField = value;
                    this.RaisePropertyChanged("CitationCount");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.Conference Conference
        {
            get
            {
                return this.ConferenceField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ConferenceField, value) != true))
                {
                    this.ConferenceField = value;
                    this.RaisePropertyChanged("Conference");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DOI
        {
            get
            {
                return this.DOIField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DOIField, value) != true))
                {
                    this.DOIField = value;
                    this.RaisePropertyChanged("DOI");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string[] FullVersionURL
        {
            get
            {
                return this.FullVersionURLField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FullVersionURLField, value) != true))
                {
                    this.FullVersionURLField = value;
                    this.RaisePropertyChanged("FullVersionURL");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint ID
        {
            get
            {
                return this.IDField;
            }
            set
            {
                if ((this.IDField.Equals(value) != true))
                {
                    this.IDField = value;
                    this.RaisePropertyChanged("ID");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.Journal Journal
        {
            get
            {
                return this.JournalField;
            }
            set
            {
                if ((object.ReferenceEquals(this.JournalField, value) != true))
                {
                    this.JournalField = value;
                    this.RaisePropertyChanged("Journal");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.Keyword[] Keyword
        {
            get
            {
                return this.KeywordField;
            }
            set
            {
                if ((object.ReferenceEquals(this.KeywordField, value) != true))
                {
                    this.KeywordField = value;
                    this.RaisePropertyChanged("Keyword");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint ReferenceCount
        {
            get
            {
                return this.ReferenceCountField;
            }
            set
            {
                if ((this.ReferenceCountField.Equals(value) != true))
                {
                    this.ReferenceCountField = value;
                    this.RaisePropertyChanged("ReferenceCount");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Title
        {
            get
            {
                return this.TitleField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TitleField, value) != true))
                {
                    this.TitleField = value;
                    this.RaisePropertyChanged("Title");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.PublicationType Type
        {
            get
            {
                return this.TypeField;
            }
            set
            {
                if ((this.TypeField.Equals(value) != true))
                {
                    this.TypeField = value;
                    this.RaisePropertyChanged("Type");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ushort Year
        {
            get
            {
                return this.YearField;
            }
            set
            {
                if ((this.YearField.Equals(value) != true))
                {
                    this.YearField = value;
                    this.RaisePropertyChanged("Year");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "PublicationType", Namespace = "http://schemas.datacontract.org/2004/07/Libra.Service.API")]
    public enum PublicationType : int
    {

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Unkown = 0,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Paper = 1,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Book = 2,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Poster = 3,
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "Request", Namespace = "http://research.microsoft.com")]
    [System.SerializableAttribute()]
    public partial class Request : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        private string AppIDField;

        private uint AuthorIDField;

        private string AuthorQueryField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.AuthorRelationshipType AuthorReltiaonshipField;

        private uint ConferenceIDField;

        private string ConferenceQueryField;

        private uint DomainIDField;

        private uint EndIdxField;

        private string FulltextQueryField;

        private uint JournalIDField;

        private string JournalQueryField;

        private uint KeywordIDField;

        private ATN.Crawler.MAS.OrderType OrderByField;

        private uint OrganizationIDField;

        private ATN.Crawler.MAS.PublicationContentType[] PublicationContentField;

        private uint PublicationIDField;

        private ATN.Crawler.MAS.ReferenceRelationship ReferenceTypeField;

        private ATN.Crawler.MAS.ObjectType ResultObjectsField;

        private uint StartIdxField;

        private uint SubDomainIDField;

        private ATN.Crawler.MAS.SuggestionType SuggestionField;

        private string TitleQueryField;

        private string VersionField;

        private short YearEndField;

        private short YearStartField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public string AppID
        {
            get
            {
                return this.AppIDField;
            }
            set
            {
                if ((object.ReferenceEquals(this.AppIDField, value) != true))
                {
                    this.AppIDField = value;
                    this.RaisePropertyChanged("AppID");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public uint AuthorID
        {
            get
            {
                return this.AuthorIDField;
            }
            set
            {
                if ((this.AuthorIDField.Equals(value) != true))
                {
                    this.AuthorIDField = value;
                    this.RaisePropertyChanged("AuthorID");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public string AuthorQuery
        {
            get
            {
                return this.AuthorQueryField;
            }
            set
            {
                if ((object.ReferenceEquals(this.AuthorQueryField, value) != true))
                {
                    this.AuthorQueryField = value;
                    this.RaisePropertyChanged("AuthorQuery");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.AuthorRelationshipType AuthorReltiaonship
        {
            get
            {
                return this.AuthorReltiaonshipField;
            }
            set
            {
                if ((this.AuthorReltiaonshipField.Equals(value) != true))
                {
                    this.AuthorReltiaonshipField = value;
                    this.RaisePropertyChanged("AuthorReltiaonship");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public uint ConferenceID
        {
            get
            {
                return this.ConferenceIDField;
            }
            set
            {
                if ((this.ConferenceIDField.Equals(value) != true))
                {
                    this.ConferenceIDField = value;
                    this.RaisePropertyChanged("ConferenceID");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public string ConferenceQuery
        {
            get
            {
                return this.ConferenceQueryField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ConferenceQueryField, value) != true))
                {
                    this.ConferenceQueryField = value;
                    this.RaisePropertyChanged("ConferenceQuery");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public uint DomainID
        {
            get
            {
                return this.DomainIDField;
            }
            set
            {
                if ((this.DomainIDField.Equals(value) != true))
                {
                    this.DomainIDField = value;
                    this.RaisePropertyChanged("DomainID");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public uint EndIdx
        {
            get
            {
                return this.EndIdxField;
            }
            set
            {
                if ((this.EndIdxField.Equals(value) != true))
                {
                    this.EndIdxField = value;
                    this.RaisePropertyChanged("EndIdx");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public string FulltextQuery
        {
            get
            {
                return this.FulltextQueryField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FulltextQueryField, value) != true))
                {
                    this.FulltextQueryField = value;
                    this.RaisePropertyChanged("FulltextQuery");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public uint JournalID
        {
            get
            {
                return this.JournalIDField;
            }
            set
            {
                if ((this.JournalIDField.Equals(value) != true))
                {
                    this.JournalIDField = value;
                    this.RaisePropertyChanged("JournalID");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public string JournalQuery
        {
            get
            {
                return this.JournalQueryField;
            }
            set
            {
                if ((object.ReferenceEquals(this.JournalQueryField, value) != true))
                {
                    this.JournalQueryField = value;
                    this.RaisePropertyChanged("JournalQuery");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public uint KeywordID
        {
            get
            {
                return this.KeywordIDField;
            }
            set
            {
                if ((this.KeywordIDField.Equals(value) != true))
                {
                    this.KeywordIDField = value;
                    this.RaisePropertyChanged("KeywordID");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public ATN.Crawler.MAS.OrderType OrderBy
        {
            get
            {
                return this.OrderByField;
            }
            set
            {
                if ((this.OrderByField.Equals(value) != true))
                {
                    this.OrderByField = value;
                    this.RaisePropertyChanged("OrderBy");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public uint OrganizationID
        {
            get
            {
                return this.OrganizationIDField;
            }
            set
            {
                if ((this.OrganizationIDField.Equals(value) != true))
                {
                    this.OrganizationIDField = value;
                    this.RaisePropertyChanged("OrganizationID");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public ATN.Crawler.MAS.PublicationContentType[] PublicationContent
        {
            get
            {
                return this.PublicationContentField;
            }
            set
            {
                if ((object.ReferenceEquals(this.PublicationContentField, value) != true))
                {
                    this.PublicationContentField = value;
                    this.RaisePropertyChanged("PublicationContent");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public uint PublicationID
        {
            get
            {
                return this.PublicationIDField;
            }
            set
            {
                if ((this.PublicationIDField.Equals(value) != true))
                {
                    this.PublicationIDField = value;
                    this.RaisePropertyChanged("PublicationID");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public ATN.Crawler.MAS.ReferenceRelationship ReferenceType
        {
            get
            {
                return this.ReferenceTypeField;
            }
            set
            {
                if ((this.ReferenceTypeField.Equals(value) != true))
                {
                    this.ReferenceTypeField = value;
                    this.RaisePropertyChanged("ReferenceType");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public ATN.Crawler.MAS.ObjectType ResultObjects
        {
            get
            {
                return this.ResultObjectsField;
            }
            set
            {
                if ((this.ResultObjectsField.Equals(value) != true))
                {
                    this.ResultObjectsField = value;
                    this.RaisePropertyChanged("ResultObjects");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public uint StartIdx
        {
            get
            {
                return this.StartIdxField;
            }
            set
            {
                if ((this.StartIdxField.Equals(value) != true))
                {
                    this.StartIdxField = value;
                    this.RaisePropertyChanged("StartIdx");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public uint SubDomainID
        {
            get
            {
                return this.SubDomainIDField;
            }
            set
            {
                if ((this.SubDomainIDField.Equals(value) != true))
                {
                    this.SubDomainIDField = value;
                    this.RaisePropertyChanged("SubDomainID");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public ATN.Crawler.MAS.SuggestionType Suggestion
        {
            get
            {
                return this.SuggestionField;
            }
            set
            {
                if ((this.SuggestionField.Equals(value) != true))
                {
                    this.SuggestionField = value;
                    this.RaisePropertyChanged("Suggestion");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public string TitleQuery
        {
            get
            {
                return this.TitleQueryField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TitleQueryField, value) != true))
                {
                    this.TitleQueryField = value;
                    this.RaisePropertyChanged("TitleQuery");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public string Version
        {
            get
            {
                return this.VersionField;
            }
            set
            {
                if ((object.ReferenceEquals(this.VersionField, value) != true))
                {
                    this.VersionField = value;
                    this.RaisePropertyChanged("Version");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public short YearEnd
        {
            get
            {
                return this.YearEndField;
            }
            set
            {
                if ((this.YearEndField.Equals(value) != true))
                {
                    this.YearEndField = value;
                    this.RaisePropertyChanged("YearEnd");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public short YearStart
        {
            get
            {
                return this.YearStartField;
            }
            set
            {
                if ((this.YearStartField.Equals(value) != true))
                {
                    this.YearStartField = value;
                    this.RaisePropertyChanged("YearStart");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "AuthorRelationshipType", Namespace = "http://schemas.datacontract.org/2004/07/Libra.Service.API")]
    public enum AuthorRelationshipType : int
    {

        [System.Runtime.Serialization.EnumMemberAttribute()]
        None = 0,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        CoAuthor = 1,
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "OrderType", Namespace = "http://schemas.datacontract.org/2004/07/Libra.Service.API")]
    public enum OrderType : int
    {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Rank = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Year = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        CitationCount = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        PublicationCount = 3,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        HIndex = 4,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        GIndex = 5,
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "PublicationContentType", Namespace = "http://schemas.datacontract.org/2004/07/Libra.Service.API")]
    public enum PublicationContentType : int
    {

        [System.Runtime.Serialization.EnumMemberAttribute()]
        AllInfo = 0,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        MetaOnly = 1,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Title = 2,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Author = 3,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Abstract = 4,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        ConferenceAndJournalInfo = 5,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        FullVersionURL = 6,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Keyword = 7,
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "ReferenceRelationship", Namespace = "http://schemas.datacontract.org/2004/07/Libra.Service.API")]
    public enum ReferenceRelationship : int
    {

        [System.Runtime.Serialization.EnumMemberAttribute()]
        None = 0,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Reference = 1,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Citation = 2,
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "ObjectType", Namespace = "http://schemas.datacontract.org/2004/07/Libra.Service.API")]
    public enum ObjectType : int
    {

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Publication = 0,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Author = 1,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Conference = 2,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Journal = 3,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Organization = 4,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Domain = 5,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Keyword = 6,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        PublicationTrend = 7,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        CitationContext = 8,
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "SuggestionType", Namespace = "http://schemas.datacontract.org/2004/07/Libra.Service.API")]
    public enum SuggestionType : int
    {

        [System.Runtime.Serialization.EnumMemberAttribute()]
        None = 0,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        NameSuggestion = 1,
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "Response", Namespace = "http://research.microsoft.com")]
    [System.SerializableAttribute()]
    public partial class Response : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.AuthorResponse AuthorField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.ConferenceResponse ConferenceField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.DomainResponse DomainField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.JournalResponse JournalField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.KeywordResponse KeywordField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.OrganizationResponse OrganizationField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.PublicationResponse PublicationField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint ResultCodeField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.TrendGraph TrendField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string VersionField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.AuthorResponse Author
        {
            get
            {
                return this.AuthorField;
            }
            set
            {
                if ((object.ReferenceEquals(this.AuthorField, value) != true))
                {
                    this.AuthorField = value;
                    this.RaisePropertyChanged("Author");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.ConferenceResponse Conference
        {
            get
            {
                return this.ConferenceField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ConferenceField, value) != true))
                {
                    this.ConferenceField = value;
                    this.RaisePropertyChanged("Conference");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.DomainResponse Domain
        {
            get
            {
                return this.DomainField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DomainField, value) != true))
                {
                    this.DomainField = value;
                    this.RaisePropertyChanged("Domain");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.JournalResponse Journal
        {
            get
            {
                return this.JournalField;
            }
            set
            {
                if ((object.ReferenceEquals(this.JournalField, value) != true))
                {
                    this.JournalField = value;
                    this.RaisePropertyChanged("Journal");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.KeywordResponse Keyword
        {
            get
            {
                return this.KeywordField;
            }
            set
            {
                if ((object.ReferenceEquals(this.KeywordField, value) != true))
                {
                    this.KeywordField = value;
                    this.RaisePropertyChanged("Keyword");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.OrganizationResponse Organization
        {
            get
            {
                return this.OrganizationField;
            }
            set
            {
                if ((object.ReferenceEquals(this.OrganizationField, value) != true))
                {
                    this.OrganizationField = value;
                    this.RaisePropertyChanged("Organization");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.PublicationResponse Publication
        {
            get
            {
                return this.PublicationField;
            }
            set
            {
                if ((object.ReferenceEquals(this.PublicationField, value) != true))
                {
                    this.PublicationField = value;
                    this.RaisePropertyChanged("Publication");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint ResultCode
        {
            get
            {
                return this.ResultCodeField;
            }
            set
            {
                if ((this.ResultCodeField.Equals(value) != true))
                {
                    this.ResultCodeField = value;
                    this.RaisePropertyChanged("ResultCode");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.TrendGraph Trend
        {
            get
            {
                return this.TrendField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TrendField, value) != true))
                {
                    this.TrendField = value;
                    this.RaisePropertyChanged("Trend");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Version
        {
            get
            {
                return this.VersionField;
            }
            set
            {
                if ((object.ReferenceEquals(this.VersionField, value) != true))
                {
                    this.VersionField = value;
                    this.RaisePropertyChanged("Version");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "TrendGraph", Namespace = "http://research.microsoft.com")]
    [System.SerializableAttribute()]
    public partial class TrendGraph : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ATN.Crawler.MAS.TrendPoint[] TrendField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ATN.Crawler.MAS.TrendPoint[] Trend
        {
            get
            {
                return this.TrendField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TrendField, value) != true))
                {
                    this.TrendField = value;
                    this.RaisePropertyChanged("Trend");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "TrendPoint", Namespace = "http://research.microsoft.com")]
    [System.SerializableAttribute()]
    public partial class TrendPoint : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint CitationCountField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint PublicationCountField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ushort YearField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint CitationCount
        {
            get
            {
                return this.CitationCountField;
            }
            set
            {
                if ((this.CitationCountField.Equals(value) != true))
                {
                    this.CitationCountField = value;
                    this.RaisePropertyChanged("CitationCount");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint PublicationCount
        {
            get
            {
                return this.PublicationCountField;
            }
            set
            {
                if ((this.PublicationCountField.Equals(value) != true))
                {
                    this.PublicationCountField = value;
                    this.RaisePropertyChanged("PublicationCount");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ushort Year
        {
            get
            {
                return this.YearField;
            }
            set
            {
                if ((this.YearField.Equals(value) != true))
                {
                    this.YearField = value;
                    this.RaisePropertyChanged("Year");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace = "http://research.microsoft.com", ConfigurationName = "MAS.IAPIService")]
    public interface IAPIService
    {

        [System.ServiceModel.OperationContractAttribute(Action = "http://research.microsoft.com/IAPIService/GetDomainList", ReplyAction = "http://research.microsoft.com/IAPIService/GetDomainListResponse")]
        ATN.Crawler.MAS.DomainResponse GetDomainList();

        [System.ServiceModel.OperationContractAttribute(Action = "http://research.microsoft.com/IAPIService/GetSubDomainList", ReplyAction = "http://research.microsoft.com/IAPIService/GetSubDomainListResponse")]
        ATN.Crawler.MAS.DomainResponse GetSubDomainList(uint domainID);

        [System.ServiceModel.OperationContractAttribute(Action = "http://research.microsoft.com/IAPIService/Search", ReplyAction = "http://research.microsoft.com/IAPIService/SearchResponse")]
        ATN.Crawler.MAS.Response Search(ATN.Crawler.MAS.Request request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://research.microsoft.com/IAPIService/GetMostViewedAuthorList", ReplyAction = "http://research.microsoft.com/IAPIService/GetMostViewedAuthorListResponse")]
        ATN.Crawler.MAS.AuthorResponse GetMostViewedAuthorList(uint domainId, uint subDomainId);

        [System.ServiceModel.OperationContractAttribute(Action = "http://research.microsoft.com/IAPIService/GetMostViewedPublicationList", ReplyAction = "http://research.microsoft.com/IAPIService/GetMostViewedPublicationListResponse")]
        ATN.Crawler.MAS.PublicationResponse GetMostViewedPublicationList(uint domainId, uint subDomainId);

        [System.ServiceModel.OperationContractAttribute(Action = "http://research.microsoft.com/IAPIService/GetPublicationByDOI", ReplyAction = "http://research.microsoft.com/IAPIService/GetPublicationByDOIResponse")]
        ATN.Crawler.MAS.PublicationResponse GetPublicationByDOI(string doi);

        [System.ServiceModel.OperationContractAttribute(Action = "http://research.microsoft.com/IAPIService/GetLatestUpdatedAuthorList", ReplyAction = "http://research.microsoft.com/IAPIService/GetLatestUpdatedAuthorListResponse")]
        ATN.Crawler.MAS.AuthorResponse GetLatestUpdatedAuthorList();

        [System.ServiceModel.OperationContractAttribute(Action = "http://research.microsoft.com/IAPIService/GetLatestUpdatedPublicationList", ReplyAction = "http://research.microsoft.com/IAPIService/GetLatestUpdatedPublicationListResponse" +
            "")]
        ATN.Crawler.MAS.PublicationResponse GetLatestUpdatedPublicationList();
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAPIServiceChannel : ATN.Crawler.MAS.IAPIService, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class APIServiceClient : System.ServiceModel.ClientBase<ATN.Crawler.MAS.IAPIService>, ATN.Crawler.MAS.IAPIService
    {

        public APIServiceClient()
        {
        }

        public APIServiceClient(string endpointConfigurationName) :
                base(endpointConfigurationName)
        {
        }

        public APIServiceClient(string endpointConfigurationName, string remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public APIServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public APIServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        public ATN.Crawler.MAS.DomainResponse GetDomainList()
        {
            return base.Channel.GetDomainList();
        }

        public ATN.Crawler.MAS.DomainResponse GetSubDomainList(uint domainID)
        {
            return base.Channel.GetSubDomainList(domainID);
        }

        public ATN.Crawler.MAS.Response Search(ATN.Crawler.MAS.Request request)
        {
            return base.Channel.Search(request);
        }

        public ATN.Crawler.MAS.AuthorResponse GetMostViewedAuthorList(uint domainId, uint subDomainId)
        {
            return base.Channel.GetMostViewedAuthorList(domainId, subDomainId);
        }

        public ATN.Crawler.MAS.PublicationResponse GetMostViewedPublicationList(uint domainId, uint subDomainId)
        {
            return base.Channel.GetMostViewedPublicationList(domainId, subDomainId);
        }

        public ATN.Crawler.MAS.PublicationResponse GetPublicationByDOI(string doi)
        {
            return base.Channel.GetPublicationByDOI(doi);
        }

        public ATN.Crawler.MAS.AuthorResponse GetLatestUpdatedAuthorList()
        {
            return base.Channel.GetLatestUpdatedAuthorList();
        }

        public ATN.Crawler.MAS.PublicationResponse GetLatestUpdatedPublicationList()
        {
            return base.Channel.GetLatestUpdatedPublicationList();
        }
    }
}
