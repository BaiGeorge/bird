// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Hitter.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Proto {

  /// <summary>Holder for reflection information generated from Hitter.proto</summary>
  public static partial class HitterReflection {

    #region Descriptor
    /// <summary>File descriptor for Hitter.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static HitterReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgxIaXR0ZXIucHJvdG8SBVByb3RvGg5FeGNoYW5nZS5wcm90bxoNUmVxdWVz",
            "dC5wcm90bxoLUmVwbHkucHJvdG8iIgoMSGl0dGVyUmVjb3JkEhIKCmluc3Ry",
            "dW1lbnQYASABKAkiJgoGSGl0dGVyEgwKBG5hbWUYASABKAkSDgoGcHJpY2Vy",
            "GAIgASgJIowBCglIaXR0ZXJSZXESIAoEdHlwZRgBIAEoDjISLlByb3RvLlJl",
            "cXVlc3RUeXBlEiEKCGV4Y2hhbmdlGAIgASgOMg8uUHJvdG8uRXhjaGFuZ2US",
            "HgoHaGl0dGVycxgDIAMoCzINLlByb3RvLkhpdHRlchIMCgRuYW1lGAQgASgJ",
            "EgwKBHVzZXIYBSABKAkiSQoJSGl0dGVyUmVwEh4KB2hpdHRlcnMYASADKAsy",
            "DS5Qcm90by5IaXR0ZXISHAoGcmVzdWx0GAIgASgLMgwuUHJvdG8uUmVwbHli",
            "BnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Proto.ExchangeReflection.Descriptor, global::Proto.RequestReflection.Descriptor, global::Proto.ReplyReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Proto.HitterRecord), global::Proto.HitterRecord.Parser, new[]{ "Instrument" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Proto.Hitter), global::Proto.Hitter.Parser, new[]{ "Name", "Pricer" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Proto.HitterReq), global::Proto.HitterReq.Parser, new[]{ "Type", "Exchange", "Hitters", "Name", "User" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Proto.HitterRep), global::Proto.HitterRep.Parser, new[]{ "Hitters", "Result" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class HitterRecord : pb::IMessage<HitterRecord> {
    private static readonly pb::MessageParser<HitterRecord> _parser = new pb::MessageParser<HitterRecord>(() => new HitterRecord());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<HitterRecord> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Proto.HitterReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HitterRecord() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HitterRecord(HitterRecord other) : this() {
      instrument_ = other.instrument_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HitterRecord Clone() {
      return new HitterRecord(this);
    }

    /// <summary>Field number for the "instrument" field.</summary>
    public const int InstrumentFieldNumber = 1;
    private string instrument_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Instrument {
      get { return instrument_; }
      set {
        instrument_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as HitterRecord);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(HitterRecord other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Instrument != other.Instrument) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Instrument.Length != 0) hash ^= Instrument.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Instrument.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Instrument);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Instrument.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Instrument);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(HitterRecord other) {
      if (other == null) {
        return;
      }
      if (other.Instrument.Length != 0) {
        Instrument = other.Instrument;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Instrument = input.ReadString();
            break;
          }
        }
      }
    }

  }

  public sealed partial class Hitter : pb::IMessage<Hitter> {
    private static readonly pb::MessageParser<Hitter> _parser = new pb::MessageParser<Hitter>(() => new Hitter());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Hitter> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Proto.HitterReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Hitter() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Hitter(Hitter other) : this() {
      name_ = other.name_;
      pricer_ = other.pricer_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Hitter Clone() {
      return new Hitter(this);
    }

    /// <summary>Field number for the "name" field.</summary>
    public const int NameFieldNumber = 1;
    private string name_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "pricer" field.</summary>
    public const int PricerFieldNumber = 2;
    private string pricer_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Pricer {
      get { return pricer_; }
      set {
        pricer_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Hitter);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Hitter other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Name != other.Name) return false;
      if (Pricer != other.Pricer) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      if (Pricer.Length != 0) hash ^= Pricer.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Name.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Name);
      }
      if (Pricer.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Pricer);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      if (Pricer.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Pricer);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Hitter other) {
      if (other == null) {
        return;
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      if (other.Pricer.Length != 0) {
        Pricer = other.Pricer;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Name = input.ReadString();
            break;
          }
          case 18: {
            Pricer = input.ReadString();
            break;
          }
        }
      }
    }

  }

  public sealed partial class HitterReq : pb::IMessage<HitterReq> {
    private static readonly pb::MessageParser<HitterReq> _parser = new pb::MessageParser<HitterReq>(() => new HitterReq());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<HitterReq> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Proto.HitterReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HitterReq() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HitterReq(HitterReq other) : this() {
      type_ = other.type_;
      exchange_ = other.exchange_;
      hitters_ = other.hitters_.Clone();
      name_ = other.name_;
      user_ = other.user_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HitterReq Clone() {
      return new HitterReq(this);
    }

    /// <summary>Field number for the "type" field.</summary>
    public const int TypeFieldNumber = 1;
    private global::Proto.RequestType type_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Proto.RequestType Type {
      get { return type_; }
      set {
        type_ = value;
      }
    }

    /// <summary>Field number for the "exchange" field.</summary>
    public const int ExchangeFieldNumber = 2;
    private global::Proto.Exchange exchange_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Proto.Exchange Exchange {
      get { return exchange_; }
      set {
        exchange_ = value;
      }
    }

    /// <summary>Field number for the "hitters" field.</summary>
    public const int HittersFieldNumber = 3;
    private static readonly pb::FieldCodec<global::Proto.Hitter> _repeated_hitters_codec
        = pb::FieldCodec.ForMessage(26, global::Proto.Hitter.Parser);
    private readonly pbc::RepeatedField<global::Proto.Hitter> hitters_ = new pbc::RepeatedField<global::Proto.Hitter>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Proto.Hitter> Hitters {
      get { return hitters_; }
    }

    /// <summary>Field number for the "name" field.</summary>
    public const int NameFieldNumber = 4;
    private string name_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "user" field.</summary>
    public const int UserFieldNumber = 5;
    private string user_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string User {
      get { return user_; }
      set {
        user_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as HitterReq);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(HitterReq other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Type != other.Type) return false;
      if (Exchange != other.Exchange) return false;
      if(!hitters_.Equals(other.hitters_)) return false;
      if (Name != other.Name) return false;
      if (User != other.User) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Type != 0) hash ^= Type.GetHashCode();
      if (Exchange != 0) hash ^= Exchange.GetHashCode();
      hash ^= hitters_.GetHashCode();
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      if (User.Length != 0) hash ^= User.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Type != 0) {
        output.WriteRawTag(8);
        output.WriteEnum((int) Type);
      }
      if (Exchange != 0) {
        output.WriteRawTag(16);
        output.WriteEnum((int) Exchange);
      }
      hitters_.WriteTo(output, _repeated_hitters_codec);
      if (Name.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(Name);
      }
      if (User.Length != 0) {
        output.WriteRawTag(42);
        output.WriteString(User);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Type != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Type);
      }
      if (Exchange != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Exchange);
      }
      size += hitters_.CalculateSize(_repeated_hitters_codec);
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      if (User.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(User);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(HitterReq other) {
      if (other == null) {
        return;
      }
      if (other.Type != 0) {
        Type = other.Type;
      }
      if (other.Exchange != 0) {
        Exchange = other.Exchange;
      }
      hitters_.Add(other.hitters_);
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      if (other.User.Length != 0) {
        User = other.User;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            type_ = (global::Proto.RequestType) input.ReadEnum();
            break;
          }
          case 16: {
            exchange_ = (global::Proto.Exchange) input.ReadEnum();
            break;
          }
          case 26: {
            hitters_.AddEntriesFrom(input, _repeated_hitters_codec);
            break;
          }
          case 34: {
            Name = input.ReadString();
            break;
          }
          case 42: {
            User = input.ReadString();
            break;
          }
        }
      }
    }

  }

  public sealed partial class HitterRep : pb::IMessage<HitterRep> {
    private static readonly pb::MessageParser<HitterRep> _parser = new pb::MessageParser<HitterRep>(() => new HitterRep());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<HitterRep> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Proto.HitterReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HitterRep() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HitterRep(HitterRep other) : this() {
      hitters_ = other.hitters_.Clone();
      Result = other.result_ != null ? other.Result.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HitterRep Clone() {
      return new HitterRep(this);
    }

    /// <summary>Field number for the "hitters" field.</summary>
    public const int HittersFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Proto.Hitter> _repeated_hitters_codec
        = pb::FieldCodec.ForMessage(10, global::Proto.Hitter.Parser);
    private readonly pbc::RepeatedField<global::Proto.Hitter> hitters_ = new pbc::RepeatedField<global::Proto.Hitter>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Proto.Hitter> Hitters {
      get { return hitters_; }
    }

    /// <summary>Field number for the "result" field.</summary>
    public const int ResultFieldNumber = 2;
    private global::Proto.Reply result_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Proto.Reply Result {
      get { return result_; }
      set {
        result_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as HitterRep);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(HitterRep other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!hitters_.Equals(other.hitters_)) return false;
      if (!object.Equals(Result, other.Result)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= hitters_.GetHashCode();
      if (result_ != null) hash ^= Result.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      hitters_.WriteTo(output, _repeated_hitters_codec);
      if (result_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Result);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += hitters_.CalculateSize(_repeated_hitters_codec);
      if (result_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Result);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(HitterRep other) {
      if (other == null) {
        return;
      }
      hitters_.Add(other.hitters_);
      if (other.result_ != null) {
        if (result_ == null) {
          result_ = new global::Proto.Reply();
        }
        Result.MergeFrom(other.Result);
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            hitters_.AddEntriesFrom(input, _repeated_hitters_codec);
            break;
          }
          case 18: {
            if (result_ == null) {
              result_ = new global::Proto.Reply();
            }
            input.ReadMessage(result_);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
