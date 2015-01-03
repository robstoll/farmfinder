package ch.tutteli.farmfinderapp;

import android.app.SearchManager;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.FragmentActivity;
import android.support.v7.app.ActionBarActivity;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.SearchView;

import com.google.android.gms.maps.CameraUpdate;
import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.SupportMapFragment;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.MarkerOptions;

import org.json.JSONArray;
import org.json.JSONObject;

public class MapsActivity extends ActionBarActivity {

    private GoogleMap map; // Might be null if Google Play services APK is not available.

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_maps);
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
            findByQuery(intent.getStringExtra(SearchManager.QUERY));
        }
    }

    private void findByQuery(String query){
        //TODO move to other class
        try {
            //TODO request farmers which are around this location by 50km and fulfill the given query
//            HttpClient client = new DefaultHttpClient();
//            HttpGet request = new HttpGet();
//            request.setURI(new URI(getString(R.string.search_api)+"/"+latitude+","+longitude+"/q="+query));
//            request.addHeader("Accept", "json/application");
//            HttpResponse response = client.execute(request);
//            if (response.getStatusLine().getStatusCode() == HttpStatus.SC_OK) {
//                JSONObject jObject = JsonHelper.InputStreamtoJson(response.getEntity().getContent());
            changeMarker(new JSONObject("{'farms':["
                    + "{'lat':'48.255856','long':'14.358788','title':'Ebelsberghof'},"
                    + "]}"));
//            }
        } catch (Exception ex) {
            //TODO exception handling
            int i = 0;
        }
    }

    private void changeMarker(JSONObject jObject){
        map.clear();
        try {
            if (jObject != null) {
                JSONArray jArray = jObject.getJSONArray("farms");
                for (int i = 0; i < jArray.length(); ++i) {
                    JSONObject jsonItem = jArray.getJSONObject(i);
                    map.addMarker(
                            new MarkerOptions()
                                    .position(new LatLng(
                                            jsonItem.getDouble("lat"),
                                            jsonItem.getDouble("long")))
                                    .title(jsonItem.getString("title")));
                }
            }
//            }
        } catch (Exception ex) {
            //TODO exception handling
            int i = 0;
        }
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

    /**
     * This is where we can add markers or lines, add listeners or move the camera. In this case, we
     * just add a marker near Africa.
     * <p/>
     * This should only be called once and when we are sure that {@link #map} is not null.
     */
    private void setUpMap() {
        //TODO source out logic to own class

        //TODO get user's location
        double latitude = 48.304238;
        double longitude = 14.287945;
        try {
            //TODO request farmers which are around this location by 50km (make 50km configurable)
//            HttpClient client = new DefaultHttpClient();
//            HttpGet request = new HttpGet();
//            request.setURI(new URI(getString(R.string.search_api)+"/"+latitude+","+longitude));
//            request.addHeader("Accept", "json/application");
//            HttpResponse response = client.execute(request);
//            if (response.getStatusLine().getStatusCode() == HttpStatus.SC_OK) {
//                JSONObject jObject = JsonHelper.InputStreamtoJson(response.getEntity().getContent());
            changeMarker(new JSONObject("{'farms':["
                    + "{'lat':'48.255856','long':'14.358788','title':'Ebelsberghof'},"
                    + "{'lat':'48.359072','long':'14.362907','title':'Müllers Bio-Rinder-Range'},"
                    + "{'lat':'48.313752','long':'14.269738','title':'Frisch von Urfahr'},"
                    + "{'lat':'48.302466','long':'14.402733','title':'Romans Bauernhof'}"
                    + "]}"));
//            }
        } catch (Exception ex) {
            //TODO exception handling
            int i = 0;
        }
        CameraUpdate center = CameraUpdateFactory.newLatLng(new LatLng(latitude, longitude));
        CameraUpdate zoom = CameraUpdateFactory.zoomTo(10);

        map.moveCamera(center);
        map.animateCamera(zoom);
    }
}
