// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable 1591 // TODO: doc comments

namespace Microsoft.Cci.Pdb
{
  internal class PdbFunction
  {
    static internal readonly Guid msilMetaData = new Guid(0xc6ea3fc9, 0x59b3, 0x49d6, 0xbc, 0x25,
                                                        0x09, 0x02, 0xbb, 0xab, 0xb4, 0x60);
    static internal readonly IComparer byAddress = new PdbFunctionsByAddress();
    static internal readonly IComparer byAddressAndToken = new PdbFunctionsByAddressAndToken();
    //static internal readonly IComparer byToken = new PdbFunctionsByToken();

    internal uint token;
    internal uint slotToken;
    internal uint tokenOfMethodWhoseUsingInfoAppliesToThisMethod;
    //internal string name;
    //internal string module;
    //internal ushort flags;

    internal uint segment;
    internal uint address;
    internal uint length;

    //internal byte[] metadata;
    internal PdbScope[] scopes;
    internal PdbSlot[] slots;
    internal PdbConstant[] constants;
    internal string[] usedNamespaces;
    internal PdbLines[] lines;
    internal ushort[]/*?*/ usingCounts;
    internal IEnumerable<INamespaceScope>/*?*/ namespaceScopes;
    internal string/*?*/ iteratorClass;
    internal List<ILocalScope>/*?*/ iteratorScopes;
    internal PdbSynchronizationInformation/*?*/ synchronizationInformation;

    /// <summary>
    /// Flag saying whether the method has been identified as a product of VB compilation using
    /// the legacy Windows PDB symbol format, in which case scope ends need to be shifted by 1
    /// due to different semantics of scope limits in VB and C# compilers.
    /// </summary>
    private bool visualBasicScopesAdjusted = false;

    private static string StripNamespace(string module)
    {
      int li = module.LastIndexOf('.');
      if (li > 0)
      {
        return module.Substring(li + 1);
      }
      return module;
    }

    /// <summary>
    /// When the Windows PDB reader identifies a PdbFunction as having 'Basic' as its source language,
    /// it calls this method which adjusts all scopes by adding 1 to their lengths to compensate
    /// for different behavior of VB vs. the C# compiler w.r.t. emission of scope info.
    /// </summary>
    internal void AdjustVisualBasicScopes()
    {
      if (!visualBasicScopesAdjusted)
      {
        visualBasicScopesAdjusted = true;

        // Don't adjust root scope as that one is correct
        foreach (PdbScope scope in scopes)
        {
          AdjustVisualBasicScopes(scope.scopes);
        }
      }
    }

    /// <summary>
    /// Recursively update the entire scope tree by adding 1 to the length of each scope.
    /// </summary>
    private void AdjustVisualBasicScopes(PdbScope[] scopes)
    {
      foreach (PdbScope scope in scopes)
      {
        scope.length++;
        AdjustVisualBasicScopes(scope.scopes);
      }
    }

    internal static PdbFunction[] LoadManagedFunctions(/*string module,*/
                                                       BitAccess bits, uint limit,
                                                       bool readStrings)
    {
      //string mod = StripNamespace(module);
      int begin = bits.Position;
      int count = 0;

      while (bits.Position < limit)
      {

        bits.ReadUInt16(out ushort siz);
        int star = bits.Position;
        int stop = bits.Position + siz;
        bits.Position = star;
        bits.ReadUInt16(out ushort rec);

        switch ((SYM)rec)
        {
          case SYM.S_GMANPROC:
          case SYM.S_LMANPROC:
            ManProcSym proc;
            bits.ReadUInt32(out proc.parent);
            bits.ReadUInt32(out proc.end);
            bits.Position = (int)proc.end;
            count++;
            break;

          case SYM.S_END:
            bits.Position = stop;
            break;

          default:
            //Console.WriteLine("{0,6}: {1:x2} {2}",
            //                  bits.Position, rec, (SYM)rec);
            bits.Position = stop;
            break;
        }
      }
      if (count == 0)
      {
        return null;
      }

      bits.Position = begin;
      PdbFunction[] funcs = new PdbFunction[count];
      int func = 0;

      while (bits.Position < limit)
      {

        bits.ReadUInt16(out ushort siz);
        int star = bits.Position;
        int stop = bits.Position + siz;
        bits.ReadUInt16(out ushort rec);

        switch ((SYM)rec)
        {

          case SYM.S_GMANPROC:
          case SYM.S_LMANPROC:
            ManProcSym proc;
            //int offset = bits.Position;

            bits.ReadUInt32(out proc.parent);
            bits.ReadUInt32(out proc.end);
            bits.ReadUInt32(out proc.next);
            bits.ReadUInt32(out proc.len);
            bits.ReadUInt32(out proc.dbgStart);
            bits.ReadUInt32(out proc.dbgEnd);
            bits.ReadUInt32(out proc.token);
            bits.ReadUInt32(out proc.off);
            bits.ReadUInt16(out proc.seg);
            bits.ReadUInt8(out proc.flags);
            bits.ReadUInt16(out proc.retReg);
            if (readStrings)
            {
              bits.ReadCString(out proc.name);
            }
            else
            {
              bits.SkipCString(out proc.name);
            }
            //Console.WriteLine("token={0:X8} [{1}::{2}]", proc.token, module, proc.name);

            bits.Position = stop;
            funcs[func++] = new PdbFunction(/*module,*/ proc, bits);
            break;

          default:
            {
              //throw new PdbDebugException("Unknown SYMREC {0}", (SYM)rec);
              bits.Position = stop;
              break;
            }
        }
      }
      return funcs;
    }

    internal static void CountScopesAndSlots(BitAccess bits, uint limit,
                                             out int constants, out int scopes, out int slots, out int usedNamespaces)
    {
      int pos = bits.Position;
      BlockSym32 block;
      constants = 0;
      slots = 0;
      scopes = 0;
      usedNamespaces = 0;

      while (bits.Position < limit)
      {

        bits.ReadUInt16(out ushort siz);
        int star = bits.Position;
        int stop = bits.Position + siz;
        bits.Position = star;
        bits.ReadUInt16(out ushort rec);

        switch ((SYM)rec)
        {
          case SYM.S_BLOCK32:
            {
              bits.ReadUInt32(out block.parent);
              bits.ReadUInt32(out block.end);

              scopes++;
              bits.Position = (int)block.end;
              break;
            }

          case SYM.S_MANSLOT:
            slots++;
            bits.Position = stop;
            break;

          case SYM.S_UNAMESPACE:
            usedNamespaces++;
            bits.Position = stop;
            break;

          case SYM.S_MANCONSTANT:
            constants++;
            bits.Position = stop;
            break;

          default:
            bits.Position = stop;
            break;
        }
      }
      bits.Position = pos;
    }

    internal PdbFunction()
    {
    }

    internal PdbFunction(/*string module, */ManProcSym proc, BitAccess bits)
    {
      token = proc.token;
      //this.module = module;
      //this.name = proc.name;
      //this.flags = proc.flags;
      segment = proc.seg;
      address = proc.off;
      length = proc.len;

      if (proc.seg != 1)
      {
        throw new PdbDebugException("Segment is {0}, not 1.", proc.seg);
      }
      if (proc.parent != 0 || proc.next != 0)
      {
        throw new PdbDebugException("Warning parent={0}, next={1}",
                                    proc.parent, proc.next);
      }
      //if (proc.dbgStart != 0 || proc.dbgEnd != 0) {
      //  throw new PdbDebugException("Warning DBG start={0}, end={1}",
      //                              proc.dbgStart, proc.dbgEnd);
      //}

      CountScopesAndSlots(bits, proc.end, out int constantCount, out int scopeCount, out int slotCount, out int usedNamespacesCount);
      int scope = constantCount > 0 || slotCount > 0 || usedNamespacesCount > 0 ? 1 : 0;
      int slot = 0;
      int constant = 0;
      int usedNs = 0;
      scopes = new PdbScope[scopeCount + scope];
      slots = new PdbSlot[slotCount];
      constants = new PdbConstant[constantCount];
      usedNamespaces = new string[usedNamespacesCount];

      if (scope > 0)
        scopes[0] = new PdbScope(address, proc.len, slots, constants, usedNamespaces);

      while (bits.Position < proc.end)
      {

        bits.ReadUInt16(out ushort siz);
        int star = bits.Position;
        int stop = bits.Position + siz;
        bits.Position = star;
        bits.ReadUInt16(out ushort rec);

        switch ((SYM)rec)
        {
          case SYM.S_OEM:
            {          // 0x0404
              OemSymbol oem;

              bits.ReadGuid(out oem.idOem);
              bits.ReadUInt32(out oem.typind);
              // internal byte[]   rgl;        // user data, force 4-byte alignment

              if (oem.idOem == msilMetaData)
              {
                string name = bits.ReadString();
                if (name == "MD2")
                {
                  ReadMD2CustomMetadata(bits);
                }
                else if (name == "asyncMethodInfo")
                {
                  synchronizationInformation = new PdbSynchronizationInformation(bits);
                }
                bits.Position = stop;
                break;
              }
              else
              {
                throw new PdbDebugException("OEM section: guid={0} ti={1}",
                                            oem.idOem, oem.typind);
                // bits.Position = stop;
              }
            }

          case SYM.S_BLOCK32:
            {
              BlockSym32 block = new BlockSym32();

              bits.ReadUInt32(out block.parent);
              bits.ReadUInt32(out block.end);
              bits.ReadUInt32(out block.len);
              bits.ReadUInt32(out block.off);
              bits.ReadUInt16(out block.seg);
              bits.SkipCString(out block.name);
              bits.Position = stop;

              scopes[scope++] = new PdbScope(address, block, bits, out slotToken);
              bits.Position = (int)block.end;
              break;
            }

          case SYM.S_MANSLOT:
            slots[slot++] = new PdbSlot(bits);
            bits.Position = stop;
            break;

          case SYM.S_MANCONSTANT:
            constants[constant++] = new PdbConstant(bits);
            bits.Position = stop;
            break;

          case SYM.S_UNAMESPACE:
            bits.ReadCString(out usedNamespaces[usedNs++]);
            bits.Position = stop;
            break;

          case SYM.S_END:
            bits.Position = stop;
            break;

          default:
            {
              //throw new PdbDebugException("Unknown SYM: {0}", (SYM)rec);
              bits.Position = stop;
              break;
            }
        }
      }

      if (bits.Position != proc.end)
      {
        throw new PdbDebugException("Not at S_END");
      }

      bits.ReadUInt16(out ushort esiz);
      bits.ReadUInt16(out ushort erec);

      if (erec != (ushort)SYM.S_END)
      {
        throw new PdbDebugException("Missing S_END");
      }
    }

    internal void ReadMD2CustomMetadata(BitAccess bits)
    {
      bits.ReadUInt8(out byte version);
      if (version == 4)
      {
        bits.ReadUInt8(out byte count);
        bits.Align(4);
        while (count-- > 0)
          ReadCustomMetadata(bits);
      }
    }

    private void ReadCustomMetadata(BitAccess bits)
    {
      int savedPosition = bits.Position;
      bits.ReadUInt8(out byte version);
      bits.ReadUInt8(out byte kind);
      bits.Position += 2;   // 2-bytes padding
      bits.ReadUInt32(out uint numberOfBytesInItem);
      if (version == 4)
      {
        switch (kind)
        {
          case 0: ReadUsingInfo(bits); break;
          case 1: ReadForwardInfo(bits); break;
          case 2: break; // this.ReadForwardedToModuleInfo(bits); break;
          case 3: ReadIteratorLocals(bits); break;
          case 4: ReadForwardIterator(bits); break;
          case 5: break; // dynamic locals - see http://index/#Microsoft.VisualStudio.LanguageServices/Shared/CustomDebugInfoReader.cs,a3031f7681d76e93
          case 6: break; // EnC data
          case 7: break; // EnC data for lambdas and closures
                         // ignore any other unknown record types that may be added in future, instead of throwing an exception
                         // see more details here: https://github.com/tmat/roslyn/blob/portable-pdb/docs/specs/PortablePdb-Metadata.md
          default: break; // throw new PdbDebugException("Unknown custom metadata item kind: {0}", kind);
        }
      }
      bits.Position = savedPosition + (int)numberOfBytesInItem;
    }

    private void ReadForwardIterator(BitAccess bits)
    {
      iteratorClass = bits.ReadString();
    }

    private void ReadIteratorLocals(BitAccess bits)
    {
      bits.ReadUInt32(out uint numberOfLocals);
      iteratorScopes = new List<ILocalScope>((int)numberOfLocals);
      while (numberOfLocals-- > 0)
      {
        bits.ReadUInt32(out uint ilStartOffset);
        bits.ReadUInt32(out uint ilEndOffset);
        iteratorScopes.Add(new PdbIteratorScope(ilStartOffset, ilEndOffset - ilStartOffset));
      }
    }

    //private void ReadForwardedToModuleInfo(BitAccess bits) {
    //}

    private void ReadForwardInfo(BitAccess bits)
    {
      bits.ReadUInt32(out tokenOfMethodWhoseUsingInfoAppliesToThisMethod);
    }

    private void ReadUsingInfo(BitAccess bits)
    {
      bits.ReadUInt16(out ushort numberOfNamespaces);
      usingCounts = new ushort[numberOfNamespaces];
      for (ushort i = 0; i < numberOfNamespaces; i++)
      {
        bits.ReadUInt16(out usingCounts[i]);
      }
    }

    internal class PdbFunctionsByAddress : IComparer
    {
      public int Compare(Object x, Object y)
      {
        PdbFunction fx = (PdbFunction)x;
        PdbFunction fy = (PdbFunction)y;

        if (fx.segment < fy.segment)
        {
          return -1;
        }
        else if (fx.segment > fy.segment)
        {
          return 1;
        }
        else if (fx.address < fy.address)
        {
          return -1;
        }
        else if (fx.address > fy.address)
        {
          return 1;
        }
        else
        {
          return 0;
        }
      }
    }

    internal class PdbFunctionsByAddressAndToken : IComparer
    {
      public int Compare(Object x, Object y)
      {
        PdbFunction fx = (PdbFunction)x;
        PdbFunction fy = (PdbFunction)y;

        if (fx.segment < fy.segment)
        {
          return -1;
        }
        else if (fx.segment > fy.segment)
        {
          return 1;
        }
        else if (fx.address < fy.address)
        {
          return -1;
        }
        else if (fx.address > fy.address)
        {
          return 1;
        }
        else
        {
          if (fx.token < fy.token)
            return -1;
          else if (fx.token > fy.token)
            return 1;
          else
            return 0;
        }
      }
    }

    //internal class PdbFunctionsByToken : IComparer {
    //  public int Compare(Object x, Object y) {
    //    PdbFunction fx = (PdbFunction)x;
    //    PdbFunction fy = (PdbFunction)y;

    //    if (fx.token < fy.token) {
    //      return -1;
    //    } else if (fx.token > fy.token) {
    //      return 1;
    //    } else {
    //      return 0;
    //    }
    //  }

    //}
  }

  internal class PdbSynchronizationInformation
  {
    internal uint kickoffMethodToken;
    internal uint generatedCatchHandlerIlOffset;
    internal PdbSynchronizationPoint[] synchronizationPoints;

    internal PdbSynchronizationInformation(BitAccess bits)
    {
      bits.ReadUInt32(out kickoffMethodToken);
      bits.ReadUInt32(out generatedCatchHandlerIlOffset);
      bits.ReadUInt32(out uint asyncStepInfoCount);
      synchronizationPoints = new PdbSynchronizationPoint[asyncStepInfoCount];
      for (uint i = 0; i < asyncStepInfoCount; i += 1)
      {
        synchronizationPoints[i] = new PdbSynchronizationPoint(bits);
      }
    }

    public uint GeneratedCatchHandlerOffset
    {
      get { return generatedCatchHandlerIlOffset; }
    }
  }

  internal class PdbSynchronizationPoint
  {
    internal uint synchronizeOffset;
    internal uint continuationMethodToken;
    internal uint continuationOffset;

    internal PdbSynchronizationPoint(BitAccess bits)
    {
      bits.ReadUInt32(out synchronizeOffset);
      bits.ReadUInt32(out continuationMethodToken);
      bits.ReadUInt32(out continuationOffset);
    }

    public uint SynchronizeOffset
    {
      get { return synchronizeOffset; }
    }

    public uint ContinuationOffset
    {
      get { return continuationOffset; }
    }
  }

}
