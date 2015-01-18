package ch.tutteli.farmfinderapp;

import android.app.AlertDialog;
import android.app.SearchManager;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.ActionBarActivity;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.SearchView;

import com.google.android.gms.maps.CameraUpdate;
import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.SupportMapFragment;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.MarkerOptions;

import org.json.JSONArray;
import org.json.JSONObject;

import ch.tutteli.farmfinderapp.bl.ApiException;
import ch.tutteli.farmfinderapp.bl.IRemoteSearch;
import ch.tutteli.farmfinderapp.bl.RemoteSearch;

public class MapsActivity extends ActionBarActivity {

    // Might be null if Google Play services APK is not available.
    private GoogleMap map;
    private IRemoteSearch remoteSearch;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_maps);
        remoteSearch = new RemoteSearch(getString(R.string.search_api));
        setUpMapIfNeeded();
        handleIntent(getIntent());
    }

    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.options_menu, menu);

        //Associate searchable configuration with the SearchView
        SearchManager searchManager =
                (SearchManager) getSystemService(Context.SEARCH_SERVICE);
        SearchView searchView =
                (SearchView) menu.findItem(R.id.search).getActionView();
        searchView.setIconifiedByDefault(false);
        searchView.setIconified(false);
        searchView.setQueryHint(getString(R.string.search_hint));
        searchView.setSearchableInfo(
                searchManager.getSearchableInfo(getComponentName()));
        //TODO provide possibility to search without query


        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
//        if (id == R.id.action_settings) {
//            return true;
//        }

        return super.onOptionsItemSelected(item);
    }

    @Override
    protected void onNewIntent(Intent intent) {
        handleIntent(intent);
    }

    private void handleIntent(Intent intent) {
        if (Intent.ACTION_SEARCH.equals(intent.getAction())) {
            find(intent.getStringExtra(SearchManager.QUERY));
        }
    }

    private void find(String query) {
        //todo get from settings
        int radius = 50;
        LatLng latLong = getCurrentPosition();
        find(latLong.latitude, latLong.longitude, radius, query);
    }

    private void find(double latitude, double longitude) {
        //todo get from settings
        int radius = 50;
        find(latitude, longitude, radius, null);
    }

    private void find(double latitude, double longitude, int radius, String query) {
        try {
            changeMarker(remoteSearch.find(latitude, longitude, radius, query));
        } catch (ApiException ex) {
            showErrorDialog(ex);
        }
    }

    private void changeMarker(JSONArray jArray) {
        map.clear();
        try {
            if (jArray != null) {
                for (int i = 0; i < jArray.length(); ++i) {
                    JSONObject jsonItem = jArray.getJSONObject(i);
                    map.addMarker(
                            new MarkerOptions()
                                    .position(new LatLng(
                                            jsonItem.getDouble("Latitude"),
                                            jsonItem.getDouble("Longitude")))
                                    .title(jsonItem.getString("Name")));
                }
            }
        } catch (Exception ex) {
            showErrorDialog(ex);
        }
    }

    private void showErrorDialog(Exception ex) {
        new AlertDialog.Builder(this)
                .setMessage(getString(R.string.dialog_unexpectedError_msg) + "\n" + ex.getMessage())
                .setTitle(R.string.dialog_unexpectedError_title)
                .setNeutralButton(android.R.string.ok, new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {
                        dialog.cancel();
                    }
                })
                .show();
    }

    @Override
    protected void onResume() {
        super.onResume();
        setUpMapIfNeeded();
    }

    /**
     * Sets up the map if it is possible to do so (i.e., the Google Play services APK is correctly
     * installed) and the map has not already been instantiated.. This will ensure that we only ever
     * call {@link #setUpMap()} once when {@link #map} is not null.
     * <p/>
     * If it isn't installed {@link SupportMapFragment} (and
     * {@link com.google.android.gms.maps.MapView MapView}) will show a prompt for the user to
     * install/update the Google Play services APK on their device.
     * <p/>
     * A user can return to this FragmentActivity after following the prompt and correctly
     * installing/updating/enabling the Google Play services. Since the FragmentActivity may not
     * have been completely destroyed during this process (it is likely that it would only be
     * stopped or paused), {@link #onCreate(Bundle)} may not be called again so we should call this
     * method in {@link #onResume()} to guarantee that it will be called.
     */
    private void setUpMapIfNeeded() {
        // Do a null check to confirm that we have not already instantiated the map.
        if (map == null) {
            // Try to obtain the map from the SupportMapFragment.
            map = ((SupportMapFragment) getSupportFragmentManager().findFragmentById(R.id.map))
                    .getMap();
            // Check if we were successful in obtaining the map.
            if (map != null) {
                setUpMap();
            }
        }
    }

    private void setUpMap() {
        LatLng latLong = getCurrentPosition();
        find(latLong.latitude, latLong.longitude);
        moveCamera(latLong.latitude, latLong.longitude);
    }

    private void moveCamera(double latitude, double longitude) {
        CameraUpdate center = CameraUpdateFactory.newLatLng(new LatLng(latitude, longitude));
        CameraUpdate zoom = CameraUpdateFactory.zoomTo(10);

        map.moveCamera(center);
        map.animateCamera(zoom);
    }

    public LatLng getCurrentPosition() {
        //TODO get current position;
        return new LatLng(48.304238, 14.287945);
    }
}
