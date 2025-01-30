package mono.com.google.android.gms.location;


public class DeviceOrientationListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.location.DeviceOrientationListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onDeviceOrientationChanged:(Lcom/google/android/gms/location/DeviceOrientation;)V:GetOnDeviceOrientationChanged_Lcom_google_android_gms_location_DeviceOrientation_Handler:Android.Gms.Location.IDeviceOrientationListenerInvoker, Xamarin.GooglePlayServices.Location\n" +
			"";
		mono.android.Runtime.register ("Android.Gms.Location.IDeviceOrientationListenerImplementor, Xamarin.GooglePlayServices.Location", DeviceOrientationListenerImplementor.class, __md_methods);
	}

	public DeviceOrientationListenerImplementor ()
	{
		super ();
		if (getClass () == DeviceOrientationListenerImplementor.class) {
			mono.android.TypeManager.Activate ("Android.Gms.Location.IDeviceOrientationListenerImplementor, Xamarin.GooglePlayServices.Location", "", this, new java.lang.Object[] {  });
		}
	}

	public void onDeviceOrientationChanged (com.google.android.gms.location.DeviceOrientation p0)
	{
		n_onDeviceOrientationChanged (p0);
	}

	private native void n_onDeviceOrientationChanged (com.google.android.gms.location.DeviceOrientation p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
