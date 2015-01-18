package ch.tutteli.farmfinderapp.bl;

import org.json.JSONArray;

public interface IRemoteSearch {
    JSONArray find(double latitude, double longitude, int radius) throws ApiException;

    JSONArray find(double latitude, double longitude, int radius, String query)
            throws ApiException;
}
