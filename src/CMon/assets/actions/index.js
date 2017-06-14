import fetch from "isomorphic-fetch"

export const REQUEST_EVENTS = "REQUEST_EVENTS";
export const RECIVE_EVENTS = "RECIVE_EVENTS";

export const requestEvents = (deviceId) => {
	return {
		type: REQUEST_EVENTS,
		deviceId
	};
};

export const reciveEvents = (deviceId, json) => {
	return {
		type: RECIVE_EVENTS,
		deviceId,
		events: json.data.children.map(child => child.data),
		receivedAt: Date.now()
	};
};

function fetchEvents(subreddit) {
	return dispatch => {
		dispatch(requestEvents(subreddit));

		return fetch(`https://www.reddit.com/r/${subreddit}.json`)
			.then(response => response.json())
			.then(json => dispatch(reciveEvents(subreddit, json)));
	};
}

function shouldFetchEvents(state, subreddit) {
	return true;
	/*const posts = state.postsBySubreddit[subreddit];
	if (!posts) {
		return true;
	} else if (posts.isFetching) {
		return false;
	} else {
		return posts.didInvalidate;
	}*/
}

export function fetchEventsIfNeeded(subreddit) {
	return (dispatch, getState) => {
		if (shouldFetchEvents(getState(), subreddit)) {
			return dispatch(fetchEvents(subreddit));
		}
	};
}

///////////////////////////////////////////////////////////

let nextTodoId = 0;

export const addTodo = (text) => {
	return {
		type: "ADD_TODO",
		id: nextTodoId++,
		text
	};
};

export const setVisibilityFilter = (filter) => {
	return {
		type: "SET_VISIBILITY_FILTER",
		filter
	};
};

export const toggleTodo = (id) => {
	return {
		type: "TOGGLE_TODO",
		id
	};
};

export const SELECT_SUBREDDIT = "SELECT_SUBREDDIT";

export function selectSubreddit(subreddit) {
	return {
		type: SELECT_SUBREDDIT,
		subreddit
	};
}

export const INVALIDATE_SUBREDDIT = "INVALIDATE_SUBREDDIT";

export function invalidateSubreddit(subreddit) {
	return {
		type: INVALIDATE_SUBREDDIT,
		subreddit
	};
}

export const REQUEST_POSTS = "REQUEST_POSTS";

function requestPosts(subreddit) {
	return {
		type: REQUEST_POSTS,
		subreddit
	};
}

export const RECEIVE_POSTS = "RECEIVE_POSTS";

function receivePosts(subreddit, json) {
	return {
		type: RECEIVE_POSTS,
		subreddit,
		posts: json.data.children.map(child => child.data),
		receivedAt: Date.now()
	};
}

// Meet our first thunk action creator!
// Though its insides are different, you would use it just like any other action creator:
// store.dispatch(fetchPosts('reactjs'))

export function fetchPosts(subreddit) {

	// Thunk middleware knows how to handle functions.
	// It passes the dispatch method as an argument to the function,
	// thus making it able to dispatch actions itself.

	return function (dispatch) {

		// First dispatch: the app state is updated to inform
		// that the API call is starting.

		dispatch(requestPosts(subreddit));

		// The function called by the thunk middleware can return a value,
		// that is passed on as the return value of the dispatch method.

		// In this case, we return a promise to wait for.
		// This is not required by thunk middleware, but it is convenient for us.

		return fetch(`https://www.reddit.com/r/${subreddit}.json`)
			.then(response => response.json())
			.then(json =>

				// We can dispatch many times!
				// Here, we update the app state with the results of the API call.

				dispatch(receivePosts(subreddit, json))
			);

		// In a real world app, you also want to
		// catch any error in the network call.
	};
}

function shouldFetchPosts(state, subreddit) {
	const posts = state.postsBySubreddit[subreddit];
	if (!posts) {
		return true;
	} else if (posts.isFetching) {
		return false;
	} else {
		return posts.didInvalidate;
	}
}

export function fetchPostsIfNeeded(subreddit) {

	// Note that the function also receives getState()
	// which lets you choose what to dispatch next.

	// This is useful for avoiding a network request if
	// a cached value is already available.

	return (dispatch, getState) => {
		if (shouldFetchPosts(getState(), subreddit)) {
			// Dispatch a thunk from thunk!
			return dispatch(fetchPosts(subreddit));
		} else {
			// Let the calling code know there's nothing to wait for.
			return Promise.resolve();
		}
	};
}
