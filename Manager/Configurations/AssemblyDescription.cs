/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetworkHost.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using CRAI.Common;

    [DataContract]
    [Serializable]
    public class AssemblyDescription : NotifyBase<AssemblyDescription>
    {
        #region public byte[] Raw
	    private byte[] _Raw;
        [DataMember]
	    public byte[] Raw
	    {
		    get { return _Raw; }
		    set 
		    { 
			    if(_Raw == value) return;
			    _Raw = value;
			    NotifyPropertyChanged();
		    }
	    }
	    #endregion public byte[] Raw
	
        #region public String Name
	    private String _Name;
        [DataMember]
	    public String Name
	    {
		    get { return _Name; }
		    set 
		    { 
			    if(_Name == value) return;
			    _Name = value;
			    NotifyPropertyChanged();
		    }
	    }
	    #endregion public String Name
	
    }
}
