// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Cash.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Proto {

  /// <summary>Holder for reflection information generated from Cash.proto</summary>
  public static partial class CashReflection {

    #region Descriptor
    /// <summary>File descriptor for Cash.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static CashReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgpDYXNoLnByb3RvEgVQcm90bxoORXhjaGFuZ2UucHJvdG8aDkN1cnJlbmN5",
            "LnByb3RvIqIBCgRDYXNoEiEKCGV4Y2hhbmdlGAEgASgOMg8uUHJvdG8uRXhj",
            "aGFuZ2USIQoIY3VycmVuY3kYAiABKA4yDy5Qcm90by5DdXJyZW5jeRIPCgdh",
            "Y2NvdW50GAMgASgJEg0KBXRvdGFsGAQgASgBEhEKCWF2YWlsYWJsZRgFIAEo",
            "ARIRCglpc19lbm91Z2gYBiABKAgSDgoGbWFyZ2luGAcgASgBYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Proto.ExchangeReflection.Descriptor, global::Proto.CurrencyReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Proto.Cash), global::Proto.Cash.Parser, new[]{ "Exchange", "Currency", "Account", "Total", "Available", "IsEnough", "Margin" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class Cash : pb::IMessage<Cash> {
    private static readonly pb::MessageParser<Cash> _parser = new pb::MessageParser<Cash>(() => new Cash());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Cash> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Proto.CashReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Cash() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Cash(Cash other) : this() {
      exchange_ = other.exchange_;
      currency_ = other.currency_;
      account_ = other.account_;
      total_ = other.total_;
      available_ = other.available_;
      isEnough_ = other.isEnough_;
      margin_ = other.margin_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Cash Clone() {
      return new Cash(this);
    }

    /// <summary>Field number for the "exchange" field.</summary>
    public const int ExchangeFieldNumber = 1;
    private global::Proto.Exchange exchange_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Proto.Exchange Exchange {
      get { return exchange_; }
      set {
        exchange_ = value;
      }
    }

    /// <summary>Field number for the "currency" field.</summary>
    public const int CurrencyFieldNumber = 2;
    private global::Proto.Currency currency_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Proto.Currency Currency {
      get { return currency_; }
      set {
        currency_ = value;
      }
    }

    /// <summary>Field number for the "account" field.</summary>
    public const int AccountFieldNumber = 3;
    private string account_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Account {
      get { return account_; }
      set {
        account_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "total" field.</summary>
    public const int TotalFieldNumber = 4;
    private double total_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public double Total {
      get { return total_; }
      set {
        total_ = value;
      }
    }

    /// <summary>Field number for the "available" field.</summary>
    public const int AvailableFieldNumber = 5;
    private double available_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public double Available {
      get { return available_; }
      set {
        available_ = value;
      }
    }

    /// <summary>Field number for the "is_enough" field.</summary>
    public const int IsEnoughFieldNumber = 6;
    private bool isEnough_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool IsEnough {
      get { return isEnough_; }
      set {
        isEnough_ = value;
      }
    }

    /// <summary>Field number for the "margin" field.</summary>
    public const int MarginFieldNumber = 7;
    private double margin_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public double Margin {
      get { return margin_; }
      set {
        margin_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Cash);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Cash other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Exchange != other.Exchange) return false;
      if (Currency != other.Currency) return false;
      if (Account != other.Account) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.Equals(Total, other.Total)) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.Equals(Available, other.Available)) return false;
      if (IsEnough != other.IsEnough) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.Equals(Margin, other.Margin)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Exchange != 0) hash ^= Exchange.GetHashCode();
      if (Currency != 0) hash ^= Currency.GetHashCode();
      if (Account.Length != 0) hash ^= Account.GetHashCode();
      if (Total != 0D) hash ^= pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.GetHashCode(Total);
      if (Available != 0D) hash ^= pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.GetHashCode(Available);
      if (IsEnough != false) hash ^= IsEnough.GetHashCode();
      if (Margin != 0D) hash ^= pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.GetHashCode(Margin);
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
      if (Exchange != 0) {
        output.WriteRawTag(8);
        output.WriteEnum((int) Exchange);
      }
      if (Currency != 0) {
        output.WriteRawTag(16);
        output.WriteEnum((int) Currency);
      }
      if (Account.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(Account);
      }
      if (Total != 0D) {
        output.WriteRawTag(33);
        output.WriteDouble(Total);
      }
      if (Available != 0D) {
        output.WriteRawTag(41);
        output.WriteDouble(Available);
      }
      if (IsEnough != false) {
        output.WriteRawTag(48);
        output.WriteBool(IsEnough);
      }
      if (Margin != 0D) {
        output.WriteRawTag(57);
        output.WriteDouble(Margin);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Exchange != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Exchange);
      }
      if (Currency != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Currency);
      }
      if (Account.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Account);
      }
      if (Total != 0D) {
        size += 1 + 8;
      }
      if (Available != 0D) {
        size += 1 + 8;
      }
      if (IsEnough != false) {
        size += 1 + 1;
      }
      if (Margin != 0D) {
        size += 1 + 8;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Cash other) {
      if (other == null) {
        return;
      }
      if (other.Exchange != 0) {
        Exchange = other.Exchange;
      }
      if (other.Currency != 0) {
        Currency = other.Currency;
      }
      if (other.Account.Length != 0) {
        Account = other.Account;
      }
      if (other.Total != 0D) {
        Total = other.Total;
      }
      if (other.Available != 0D) {
        Available = other.Available;
      }
      if (other.IsEnough != false) {
        IsEnough = other.IsEnough;
      }
      if (other.Margin != 0D) {
        Margin = other.Margin;
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
            exchange_ = (global::Proto.Exchange) input.ReadEnum();
            break;
          }
          case 16: {
            currency_ = (global::Proto.Currency) input.ReadEnum();
            break;
          }
          case 26: {
            Account = input.ReadString();
            break;
          }
          case 33: {
            Total = input.ReadDouble();
            break;
          }
          case 41: {
            Available = input.ReadDouble();
            break;
          }
          case 48: {
            IsEnough = input.ReadBool();
            break;
          }
          case 57: {
            Margin = input.ReadDouble();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
