package ch.tutteli.farmfinderapp.bl;

public class ApiException extends Exception {

    public ApiException(String detailMessage) {
        super(detailMessage);
    }

    public ApiException(Throwable throwable) {
        super(throwable);
    }
}
