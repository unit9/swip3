package com.facebook.unity;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;

public class FBUnityLoginActivity extends Activity{
	public static final String LOGIN_PARAMS = "login_params";

	@Override
	protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        FB.LoginUsingActivity(getIntent().getStringExtra(LOGIN_PARAMS), this);
	}

	@Override
	protected void onStart() {
		super.onStart();
	}

	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) {
		super.onActivityResult(requestCode, resultCode, data);
		FB.onActivityResult(this, requestCode, resultCode, data);
	}
}
