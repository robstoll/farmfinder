package ch.tutteli.farmfinderapp.bl;

import android.os.AsyncTask;

import org.apache.http.HttpResponse;
import org.apache.http.HttpStatus;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.DefaultHttpClient;
import org.json.JSONArray;

import java.net.URI;
import java.net.URISyntaxException;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.TimeoutException;

public class RemoteSearch implements IRemoteSearch {

    private String apiUrl;

    public RemoteSearch(String theApiUrl) {
        apiUrl = theApiUrl;
    }

    public JSONArray find(double latitude, double longitude, int radius) throws ApiException {
        return find(latitude, longitude, radius, null);
    }

    public JSONArray find(double latitude, double longitude, int radius, String query)
            throws ApiException {
        JSONArray result = null;

        String uri = apiUrl
                + "?Latitude=" + latitude
                + "&Longitude=" + longitude
                + "&Radius=" + radius;
        if (query != null) {
            uri += "&Query=" + query;
        }

        try {
            RemoteSearchAsync async = new RemoteSearchAsync();
            AsyncTask<URI, Void, JSONArray> task = async.execute(new URI(uri));
            result = task.get(2000, TimeUnit.MILLISECONDS);
            if (async.exception != null) {
                throw async.exception;
            }
        } catch (URISyntaxException ex) {
            throw new ApiException(ex);
        } catch (InterruptedException | ExecutionException | TimeoutException e) {
            throw new ApiException(e);
        }

        return result;
    }

    private class RemoteSearchAsync extends AsyncTask<URI, Void, JSONArray> {

        public ApiException exception;

        @Override
        protected JSONArray doInBackground(URI... uris) {
            JSONArray result = null;
            try {
                HttpClient client = new DefaultHttpClient();
                HttpGet request = new HttpGet();

                request.setURI(uris[0]);
                request.addHeader("Accept", "json/application");
                HttpResponse response = client.execute(request);
                if (response.getStatusLine().getStatusCode() == HttpStatus.SC_OK) {
                    result = JsonHelper.InputStreamToJsonArray(response.getEntity().getContent());
                } else {
                    throw new RuntimeException(response.getStatusLine().getStatusCode() + " "
                            + response.getStatusLine().getReasonPhrase());
                }
            } catch (Exception ex) {
                exception = new ApiException(ex);
            }
            return result;
        }
    }
}
